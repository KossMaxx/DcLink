using Dapper;
using LegacySql.Data;
using LegacySql.Domain.IncomingBills;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.IncomingBills
{
    public class IncomingBillRepository : ILegacyIncomingBillRepository
    {
        private readonly LegacyDbContext _db;
        private readonly LegacyDbConnection _connection;
        private readonly AppDbContext _mapDb;
        private readonly IProductMappingResolver _productMappingResolver;
        private readonly int _notFullMappingFilterPortion;
        public IncomingBillRepository(
            LegacyDbContext db, 
            LegacyDbConnection connection, 
            AppDbContext mapDb, 
            IProductMappingResolver productMappingResolver, 
            int notFullMappingFilterPortion)
        {
            _db = db;
            _connection = connection;
            _mapDb = mapDb;
            _productMappingResolver = productMappingResolver;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
        }

        private async IAsyncEnumerable<IncomingBill> GetAllAsync(Expression<Func<IncomingBillEF, bool>> filter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var billsQuery = _db.IncomingBills
                .FromSqlRaw($@"select 
                                bill.[Код_счета], 
                                bill.[Номер], 
                                bill.[дата], 
                                bill.[DataLastChange], 
                                bill.[OOO], 
                                bill.[FirmID],
                                bill.[klientID]
                                from [dbo].[СчетПН] bill
                                left join [dbo].[connected_documents] cd on ([type1] =  11 and [type2] = 7 and [doc1ID] = bill.[Код_счета]) or ([type1] =  7 and [type2] = 11 and [doc2ID] = bill.[Код_счета])
                                left join [dbo].[platezka] pl on (pl.[bank_ID] = cd.[doc2ID] and cd.[type2]=7) or (pl.[bank_ID] = cd.[doc1ID] and cd.[type1]=7)
                                where [Возврат] = 0 and (cast(bill.[дата] as date) >= cast('2022-05-01' as date) or cast(bill.[DataLastChange] as date) >= cast('2022-05-01' as date))
                                and ((select count(*) from [dbo].[connected_documents] 
	                                   where ([type1] = 11 and [type2] = 7 and [doc1ID] = bill.[Код_счета]) or ([type1] = 7 and [type2] = 11 and [doc2ID] = bill.[Код_счета])) = 0
                                or (isnull(pl.[sum], 0) < isnull(bill.[Сумма], 0) or pl.[sum] is null))")
                .Include(e=>e.Supplier)
                .AsQueryable();

            if (filter != null)
            {
                billsQuery = billsQuery.Where(filter);
            }
#if DEBUG
            billsQuery = billsQuery.Take(1000);
#endif
            var bills = (await billsQuery.ToListAsync(cancellationToken))
                .GroupBy(e=>e.Id)
                .Select(e=>e.First());

            await foreach (var clientOrder in MapToDomain(bills, cancellationToken))
            {
                yield return clientOrder;
            }
        }

        public async IAsyncEnumerable<IncomingBill> GetChangedIncomingBillOrdersAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken)
        {
            Expression<Func<IncomingBillEF, bool>> filter = PredicateBuilder.New<IncomingBillEF>(true);
            if (changedAt.HasValue)
            {
                filter = filter.And(p => p.ChangedAt > changedAt);
            }

            await foreach (var mappedItem in GetAllAsync(filter, cancellationToken))
            {
                yield return mappedItem;
            }

            await foreach (var mappedItem in GetByNotFullMapping(notFullMappingIds, cancellationToken))
            {
                yield return mappedItem;
            }
        }

        public async IAsyncEnumerable<IncomingBill> GetIncomingBillAsync(int id, CancellationToken cancellationToken)
        {
            Expression<Func<IncomingBillEF, bool>> filter = PredicateBuilder.New<IncomingBillEF>(e=>e.Id == id);
            await foreach (var mappedItem in GetAllAsync(filter, cancellationToken))
            {
                yield return mappedItem;
            }
        }

        private async IAsyncEnumerable<IncomingBill> MapToDomain(IEnumerable<IncomingBillEF> billsEf, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var p in billsEf)
            {
                var product = await MapToDomain(p, cancellationToken);
                if (product != null)
                {
                    yield return product;
                }
            }
        }

        private async Task<IncomingBill> MapToDomain(IncomingBillEF billEF, CancellationToken cancellationToken)
        {
            var billMap = await _mapDb.IncomingBilsMaps.AsNoTracking().
                FirstOrDefaultAsync(b => b.LegacyId == billEF.Id, cancellationToken);
            var clientMap = billEF.ClientId.HasValue
                ? await _mapDb.ClientMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == billEF.ClientId, cancellationToken: cancellationToken)
                : null;

            IEnumerable<IncomingBillItem> items = await GetItems(billEF.Id, cancellationToken);

            return new IncomingBill(
                hasMap: billMap != null,
                id: new IdMap(billEF.Id, billMap?.ErpGuid),
                date: billEF.Date,
                incominNumber: billEF.IncomingNumber,
                recipientOkpo: await GetRecipientOkpo(billEF.RecipientId, cancellationToken),
                supplierOkpo: billEF.SupplierId.HasValue
                ? billEF.Supplier.TaxCode
                : null,
                supplierSqlId: billEF.SupplierId.HasValue
                ? billEF.SupplierId
                : null,
                clientId: billEF.ClientId.HasValue
                ? new IdMap(billEF.ClientId.Value, clientMap?.ErpGuid)
                : null,
                changedAt: billEF.ChangedAt,
                purchaseId: await GetPurchaseId(billEF.Id, cancellationToken),
                items: items
                );
        }

        private async Task<IdMap> GetPurchaseId(int billId, CancellationToken cancellationToken)
        {
            var bills = new List<int>();
            var connectedDocuments = await _db.ConnectedDocuments
                .Where(e => (e.Doc2Id == billId && e.Type1 == 3 && e.Type2 == 11)
                                          || (e.Doc1Id == billId && e.Type1 == 11 && e.Type2 == 3))
                .FirstOrDefaultAsync(cancellationToken);

            if (connectedDocuments == null)
            {
                return null;
            }

            var purchaseSqlId = connectedDocuments.Type1 == 3 ? connectedDocuments.Doc1Id : connectedDocuments.Doc2Id;
            var purchaseMap = await _mapDb.PurchaseMaps.AsNoTracking().FirstOrDefaultAsync(e=>e.LegacyId == purchaseSqlId, cancellationToken);

            return new IdMap(purchaseSqlId, purchaseMap?.ErpGuid);
        }

        private async Task<string> GetRecipientOkpo(int id, CancellationToken cancellationToken)
        {
            var sqlQuery = @"select okpo from [dbo].[OOO]
                            where ID = @Id";

            using (var connection = _connection.Connection)
            {
                var result = await connection.QueryAsync<string>(sqlQuery, new
                {
                    Id = id
                });
                return result.FirstOrDefault();
            }
        }

        private async Task<IEnumerable<IncomingBillItem>> GetItems(int billSqlId, CancellationToken cancellationToken)
        {
            var items = await _db.IncomingBillItems
                .Include(e => e.Nomenclature)
                .Where(e => e.IncomingBillId == billSqlId)
                .Select(e => new
                {
                    NomenclatureId = e.NomenclatureId,
                    Nomenclature = e.Nomenclature == null
                            ? null
                            : new
                            {
                                e.Nomenclature.NonCashProductId
                            },
                    e.Quantity,
                    e.PriceUAH
                })
                .ToListAsync(cancellationToken);

            if (!items.Any())
            {
                return new List<IncomingBillItem>();
            }

            var billItems = new List<IncomingBillItem>();
            foreach (var item in items)
            {
                var nomenclatureMap = item.NomenclatureId.HasValue ? await GetProductMapping(item.NomenclatureId.Value,
                        item.Nomenclature?.NonCashProductId,
                        cancellationToken)
                : null;
                billItems.Add(new IncomingBillItem(
                item.PriceUAH ?? 0,
                item.Quantity ?? 0,
                nomenclatureMap
                ));
            }

            return billItems;
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            return new IdMap(productMainSqlId, productErpGuid);
        }

        private async IAsyncEnumerable<IncomingBill> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
                yield break;
            }

            var notFullMappingCount = notFullMappingIds.Count();
            var cycleLimitation = (double)notFullMappingCount / _notFullMappingFilterPortion;
            for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
            {
                Expression<Func<IncomingBillEF, bool>> filter =
                    p => notFullMappingIds.Skip(i * _notFullMappingFilterPortion)
                        .Take(_notFullMappingFilterPortion).Any(id => id == p.Id);

                await foreach (var mappedItem in GetAllAsync(filter, cancellationToken))
                {
                    yield return mappedItem;
                }
            }
        }
    }
}
