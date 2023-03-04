using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Data.Models;
using LegacySql.Domain.Products;
using LegacySql.Domain.Purchases;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class PurchaseRepository : ILegacyPurchaseRepository
    {
        private readonly LegacyDbContext _db;
        private readonly LegacyDbConnection _connection;
        private readonly AppDbContext _mapDb;
        private readonly int _notFullMappingFilterPortion;
        private readonly IProductMappingResolver _productMappingResolver;
        private Dictionary<int, IdMap> _warehouseMaps = new Dictionary<int, IdMap>();
        private Dictionary<int, IdMap> _clientMaps = new Dictionary<int, IdMap>();
        private Dictionary<(int, int?), IdMap> _productMaps = new Dictionary<(int, int?), IdMap>();

        public PurchaseRepository(LegacyDbContext db, AppDbContext mapDb, int notFullMappingFilterPortion, IProductMappingResolver productMappingResolver, LegacyDbConnection connection)
        {
            _db = db;
            _mapDb = mapDb;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
            _productMappingResolver = productMappingResolver;
            _connection = connection;
        }

        private async IAsyncEnumerable<Purchase> GetAllAsync(Expression<Func<PurchaseEF, bool>> filter, int? take,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var purchasesQuery = _db.Purchases
                .FromSqlRaw($@"select pn.* from [dbo].[ПН] pn 
                                inner join [dbo].[Клиенты] cl on cl.[КодПоставщика] = pn.[klientID]
                                where [тип] = {(int)PurchaseType.PurchaseProducts}
                                and cl.[department] != 42 and cl.[КодПоставщика] != 25081
                                and cast(pn.[Дата] as date) >= '2022-05-1'")
                .Include(p => p.Items).ThenInclude(i => i.Product)
                .AsQueryable();

            if (filter != null)
            {
                purchasesQuery = purchasesQuery.Where(filter);
            }

#if DEBUG
            purchasesQuery = purchasesQuery.Take(1000);
#endif

            var purchasesEf = await purchasesQuery.ToListAsync(cancellationToken);

            await foreach (var i in MapToDomain(purchasesEf, cancellationToken))
            {
                yield return i;
            }
        }

        public async IAsyncEnumerable<Purchase> GetChangedAsync(DateTime? changedAt,
            IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Expression<Func<PurchaseEF, bool>> filter = PredicateBuilder.New<PurchaseEF>(true);
            if (changedAt.HasValue)
            {
                filter = filter.And(p => p.ChangedAt.HasValue && p.ChangedAt > changedAt);
            }

            var changedBillsOrderIds = (await GetChangedBillsOrderIds(changedAt.Value, cancellationToken)).ToList();
            if (changedBillsOrderIds.Any())
            {
                filter = filter.And(p=>changedBillsOrderIds.Any(e=>e == p.Id));
            }


            await foreach (var mappedItem in GetAllAsync(filter, null, cancellationToken))
            {
                yield return mappedItem;
            }

            await foreach (var mappedItem in GetByNotFullMapping(notFullMappingIds, cancellationToken))
            {
                yield return mappedItem;
            }
        }

        public async IAsyncEnumerable<Purchase> GetPurchaseAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Expression<Func<PurchaseEF, bool>> filter = p => p.Id == id;
            await foreach (var mappedItem in GetAllAsync(filter, null, cancellationToken))
            {
                yield return mappedItem;
            }
        }

        public async IAsyncEnumerable<Purchase> GetOpenAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Expression<Func<PurchaseEF, bool>> filter = PredicateBuilder.New<PurchaseEF>(true);
            await foreach (var mappedItem in GetAllAsync(filter, null, cancellationToken))
            {
                yield return mappedItem;
            }
        }

        private async IAsyncEnumerable<Purchase> MapToDomain(IEnumerable<PurchaseEF> purchasesEf,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var p in purchasesEf)
            {
                var purchase = await MapToDomain(p, cancellationToken);
                if (purchase != null)
                {
                    yield return purchase;
                }
            }
        }

        private async Task<Purchase> MapToDomain(PurchaseEF purchaseEf, CancellationToken cancellationToken)
        {
            var okpo = await GetOkpo(purchaseEf.Id, cancellationToken);

            var warehouseMap = purchaseEf.WarehouseId.HasValue
                ? await GetMap<WarehouseMapEF>(purchaseEf.WarehouseId.Value, _warehouseMaps, cancellationToken)
                : null;
            var clientMap = await GetMap<ClientMapEF>(purchaseEf.ClientId, _clientMaps, cancellationToken);

            var purchaseEfMap = await _mapDb.PurchaseMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == purchaseEf.Id, cancellationToken);

            var (billIds, firmId) = await GetBills(purchaseEf.Id, cancellationToken);

            return new Purchase(
                purchaseEfMap != null,
                new IdMap(purchaseEf.Id, purchaseEfMap?.ErpGuid),
                purchaseEf.Id,
                purchaseEf.Date,
                purchaseEf.IsExecuted,
                purchaseEf.Comments,
                (PurchaseType)purchaseEf.Type,
                purchaseEf.IsActual,
                purchaseEf.TransportationCost,
                purchaseEf.CostType,
                purchaseEf.IsApproved,
                purchaseEf.IsFinancialSideConfirmed,
                purchaseEf.IsProductsArrivedToPort,
                purchaseEf.IsCashlessDocumentsProcessNeeded,
                purchaseEf.SupplierDocument,
                purchaseEf.ShippingDate,
                purchaseEf.ChangedAt,
                warehouseMap,
                clientMap,
                purchaseEf.IsPaid,
                GetPurchaseItems(purchaseEf, cancellationToken),
                purchaseEf.EmployeeUsername,
                purchaseEf.PaymentDate,
                okpo,
                firmId,
                billIds
            );
        }

        private async Task<string> GetOkpo(int id, CancellationToken cancellationToken)
        {
            var documents = await _db.ConnectedDocuments
                .Where(d => d.Type1 == 11 && d.Type2 == 3 && d.Doc2Id == id)
                .ToListAsync(cancellationToken);

            if (!documents.Any() || documents.Count() > 1)
            {
                return null;
            }

            var sqlQuery = @"select okpo from [dbo].[OOO]
                              where ID in 
                              (select OOO from [dbo].[СчетПН]
                              where Код_счета in (@Id))";

            var result = await _connection.Connection.QueryAsync<string>(sqlQuery, new
            {
                Id = documents.First().Doc1Id
            });

            return result.FirstOrDefault();
        }

        private IEnumerable<PurchaseItem> GetPurchaseItems(PurchaseEF purchaseEf, CancellationToken cancellationToken)
        {
            return purchaseEf.Items.Select(async i => new PurchaseItem(
                    i.Price,
                    i.Quantity,
                    i.ProductId > 0
                    ? await GetProductMapping(
                        i.ProductId.Value,
                        i.Product?.NonCashProductId,
                        cancellationToken)
                    : null))
                .Select(p => p.Result)
                .ToList();
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            if (_productMaps.TryGetValue((productId, productCashlessId), out var map))
            {
                return map;
            }

            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            map = new IdMap(productMainSqlId, productErpGuid);
            _productMaps.Add((productId, productCashlessId), map);
            return map;
        }

        private async IAsyncEnumerable<Purchase> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
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
                Expression<Func<PurchaseEF, bool>> filter =
                    p => notFullMappingIds.Skip(i * _notFullMappingFilterPortion)
                        .Take(_notFullMappingFilterPortion).Any(id => id == p.Id);

                await foreach (var mappedItem in GetAllAsync(filter, null, cancellationToken))
                {
                    yield return mappedItem;
                }
            }
        }

        private async Task<IdMap> GetMap<T>(int id, Dictionary<int, IdMap> maps, CancellationToken cancellationToken)
            where T : BaseMapModel
        {
            if (id <= 0)
            {
                return null;
            }

            if (maps.TryGetValue(id, out var map))
            {
                return map;
            }

            var mapEf = await _mapDb.Set<T>().AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == id, cancellationToken);
            map = new IdMap(id, mapEf?.ErpGuid);
            maps.Add(id, map);
            return map;
        }

        private async Task<(IEnumerable<int> billIds, int? firmId)> GetBills(int orderId, CancellationToken cancellationToken)
        {
            var billIds = new List<int>();
            var connectedDocuments = await _db.ConnectedDocuments
                .Where(e => (e.Doc1Id == orderId && e.Type1 == 3 && e.Type2 == 11)
                                          || (e.Doc2Id == orderId && e.Type1 == 11 && e.Type2 == 3))
                .ToListAsync(cancellationToken);

            if (connectedDocuments == null)
            {
                return (billIds, null);
            }

            billIds.AddRange(connectedDocuments.Where(e=>e.Type1 == 3).Select(e=>e.Doc2Id));
            billIds.AddRange(connectedDocuments.Where(e => e.Type2 == 3).Select(e => e.Doc1Id));

            var bills = await _db.IncomingBills.Where(e => billIds.Contains(e.Id)).ToListAsync(cancellationToken);

            return (billIds, bills.FirstOrDefault(e=>e.SupplierId.HasValue)?.SupplierId);
        }

        private async Task<IEnumerable<int>> GetChangedBillsOrderIds(DateTime changetAt, CancellationToken cancellationToken)
        {
            var connectedDocuments = await _db.ConnectedDocuments
                .Where(e => e.Date.HasValue && e.Date > changetAt && ((e.Type1 == 3 && e.Type2 == 11)
                                          || (e.Type1 == 11 && e.Type2 == 3)))
                .ToListAsync(cancellationToken);

            if (!connectedDocuments.Any())
            {
                return new LinkedList<int>();
            }

            return connectedDocuments.Select(e => e.Type1 == 3 ? e.Doc1Id : e.Doc2Id);
        }
    }
}