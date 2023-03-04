using Dapper;
using LegacySql.Data;
using LegacySql.Data.Models;
using LegacySql.Domain.Bills;
using LegacySql.Domain.Deliveries.Inner;
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

namespace LegacySql.Legacy.Data.Bills
{
    public class BillRepository : ILegacyBillRepository
    {
        private readonly LegacyDbContext _db;
        private readonly LegacyDbConnection _connection;
        private readonly AppDbContext _mapDb;
        private readonly IProductMappingResolver _productMappingResolver;
        private readonly int _notFullMappingFilterPortion;

        public BillRepository(
            LegacyDbContext db,
            AppDbContext mapDb,
            int notFullMappingFilterPortion,
            IProductMappingResolver productMappingResolver,
            LegacyDbConnection connection)
        {
            _db = db;
            _mapDb = mapDb;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
            _productMappingResolver = productMappingResolver;
            _connection = connection;
        }

        private async IAsyncEnumerable<Bill> GetAllAsync(Expression<Func<BillEF, bool>> filter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var billsQuery = _db.Bills
                .FromSqlRaw($@"select * from [dbo].[Счет] bill
                              where [ValidTo] >= CAST( GETDATE() AS Date ) 
                              and ([оплачен] is null or [оплачен]=0)
                              and ([выдан] is null or [выдан]=0)
                              and (select count(*) from [dbo].[connected_documents] 
                              where (type1 = 10 and type2 = 1 and doc1ID=bill.[Код_счета]) 
                              or (type1 = 1 and type2 = 10 and doc2ID=bill.[Код_счета]))=0")
                .Include(e => e.Client)
                .Include(e => e.Firm)
                .AsQueryable();

            if (filter != null)
            {
                billsQuery = billsQuery.Where(filter);
            }
#if DEBUG
            billsQuery = billsQuery.Take(1000);
#endif
            var bills = await billsQuery.ToListAsync(cancellationToken);

            await foreach (var clientOrder in MapToDomain(bills, cancellationToken))
            {
                yield return clientOrder;
            }
        }

        public async IAsyncEnumerable<Bill> GetChangedBillOrdersAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Expression<Func<BillEF, bool>> filter = PredicateBuilder.New<BillEF>(true);
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

        public async Task<Bill> GetBillAsync(int id, CancellationToken cancellationToken)
        {
            var billEF = await _db.Bills
                .FromSqlRaw(@$"select * from [dbo].[Счет]
                            where [Код_счета] = {id}")
                .Include(e => e.Client)
                .Include(e=>e.Firm)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (billEF == null)
            {
                return null;
            }

            return await MapToDomain(billEF, cancellationToken);
        }

        private async Task<Bill> MapToDomain(BillEF billEF, CancellationToken cancellationToken)
        {
            var billMap = await _mapDb.BillMaps.AsNoTracking().
                FirstOrDefaultAsync(b => b.LegacyId == billEF.Id, cancellationToken);
            var clientMap = billEF.ClientId.HasValue
                ? await _mapDb.ClientMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == billEF.ClientId, cancellationToken: cancellationToken)
                : null;

            EmployeeMapEF creatorMap = null;
            var legacyCreator = string.IsNullOrEmpty(billEF.Creator)
                ? null
                : await _db.Employees.FirstOrDefaultAsync(e => e.NickName == billEF.Creator, cancellationToken: cancellationToken);
            if (legacyCreator != null)
            {
                creatorMap = await _mapDb.EmployeeMaps.AsNoTracking()
                    .FirstOrDefaultAsync(em => em.LegacyId == legacyCreator.Id, cancellationToken: cancellationToken);
            }

            EmployeeMapEF managerMap = null;
            var legacyManager = !billEF.ManagerId.HasValue
                ? null
                : await _db.Employees.FirstOrDefaultAsync(e => e.Id == billEF.ManagerId, cancellationToken: cancellationToken);
            if (legacyManager != null)
            {
                managerMap = await _mapDb.EmployeeMaps.AsNoTracking()
                    .FirstOrDefaultAsync(em => em.LegacyId == legacyManager.Id, cancellationToken: cancellationToken);
            }

            (IEnumerable<BillItem> items, decimal totalUah, decimal total) = await GetItems(billEF.Id, billEF.Rate, cancellationToken);
            if (!items.Any())
            {
                (totalUah, total) = await GetDataWithoutProduct(billEF.Id, billEF.Rate, cancellationToken);
            }

            return new Bill(
                id: new IdMap(billEF.Id, billMap?.ErpGuid),
                date: billEF.Date,
                clientId: billEF.ClientId.HasValue
                ? new IdMap(billEF.ClientId.Value, clientMap?.ErpGuid)
                : null,
                comments: billEF.Comments,
                changedAt: billEF.ChangedAt,
                sellerOkpo: billEF.Seller.HasValue
                ? await GetSellerOkpo((int)billEF.Seller, cancellationToken)
                : null,
                validToDate: billEF.ValidToDate,
                firmOkpo: billEF.FirmId.HasValue && billEF.FirmId > 0
                ? billEF.Firm.TaxCode
                : null,
                firmSqlId: billEF.FirmId,
                creatorId: string.IsNullOrEmpty(billEF.Creator) || legacyCreator == null
                ? null
                : new IdMap(legacyCreator.Id, creatorMap?.ErpGuid),
                managerId: !billEF.ManagerId.HasValue || legacyManager == null
                ? null
                : new IdMap(legacyManager.Id, managerMap?.ErpGuid),
                number: billEF.Number,
                issued: billEF.Issued,
                items: items,
                quantity: items.Sum(x => x.Quantity),
                amount: items.Sum(e => e.PriceUAH),
                total: total,
                totalUah: totalUah,
                hasMap: billMap != null,
                delivery: await GetDelivery(billEF, cancellationToken)
                );
        }

        private async Task<(decimal totalUah, decimal total)> GetDataWithoutProduct(int billSqlId, float? rate, CancellationToken cancellationToken)
        {
            var items = await _db.BillItems
                .Where(e => e.BillId == billSqlId)
                .Select(e => new
                {
                    e.Amount,
                    e.AmountUAH
                })
                .ToListAsync(cancellationToken);

            if (!items.Any())
            {
                return (0, 0);
            }

            var totalUah = items.Sum(x => x.AmountUAH);
            var total = items.Sum(x => x.Amount * (decimal)rate);

            return (totalUah ?? 0, total ?? 0);
        }

        private async IAsyncEnumerable<Bill> MapToDomain(IEnumerable<BillEF> billsEf, [EnumeratorCancellation] CancellationToken cancellationToken)
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

        private async Task<string> GetSellerOkpo(int id, CancellationToken cancellationToken)
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

        private async Task<(IEnumerable<BillItem>, decimal totalUah, decimal total)> GetItems(int billSqlId, float? rate, CancellationToken cancellationToken)
        {
            var items = await _db.BillItems
                .Include(e => e.Nomenclature)
                .Where(e => e.BillId == billSqlId)
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
                    e.Price,
                    e.PriceUAH,
                    Warranty = e.Nomenclature == null ? null : e.Nomenclature.Guarantee,
                    e.Amount,
                    e.AmountUAH
                })
                .ToListAsync(cancellationToken);

            if (!items.Any())
            {
                return (new List<BillItem>(), 0, 0);
            }

            var totalUah = items.Sum(x => x.AmountUAH);
            var total = items.Sum(x => x.Amount * (decimal)rate);

            var billItems = items.Select(async item =>
                new BillItem(
                await GetProductMapping(item.NomenclatureId,
                        item.Nomenclature?.NonCashProductId,
                        cancellationToken),
                item.Quantity ?? 0,
                item.Price,
                item.PriceUAH,
                item.Warranty ?? 0)
            ).Select(e => e.Result);

            return (billItems, totalUah ?? 0, total ?? 0);
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            return new IdMap(productMainSqlId, productErpGuid);
        }

        private async IAsyncEnumerable<Bill> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
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
                Expression<Func<BillEF, bool>> filter =
                    p => notFullMappingIds.Skip(i * _notFullMappingFilterPortion)
                        .Take(_notFullMappingFilterPortion).Any(id => id == p.Id);

                await foreach (var mappedItem in GetAllAsync(filter, cancellationToken))
                {
                    yield return mappedItem;
                }
            }
        }

        private async Task<Delivery> GetDelivery(BillEF bill, CancellationToken cancellationToken)
        {
            var selectDeliveryIdSqlQuery = @"select [box_is] from [dbo].[PI_BOX]
                                            where [PI_id] = @BillId";
            int? deliveryId = null;
            using (var connection = _connection.Connection)
            {
                deliveryId = await connection.QueryFirstOrDefaultAsync<int?>(selectDeliveryIdSqlQuery, new
                {
                    BillId = bill.Id
                });
            }                

            if (deliveryId == null)
            {
                return null;
            }

            var delivery = await _db.Deliveries.FirstOrDefaultAsync(
                e => e.Id == deliveryId, cancellationToken);

            if (delivery == null)
            {
                return null;
            }

            NewPostCityEF newPostCity = null;
            if (delivery.RecipientCityId.HasValue)
            {
                newPostCity = await _db.NewPostCities.FirstOrDefaultAsync(e => e.CityRef == delivery.RecipientCityId,
                    cancellationToken);
            }

            DeliveryTypeEF deliveryType = null;
            if (delivery.TypeId.HasValue)
            {
                deliveryType = await _db.DeliveryTypes.Include(e => e.CarrierType)
                    .FirstOrDefaultAsync(e => e.Id == delivery.TypeId, cancellationToken);
            }

            var recipient = new DeliveryRecipient(
                    delivery.RecipientName,
                    delivery.RecipientPhone,
                    new DeliveryRecipientAddress(delivery.RecipientAddress, newPostCity?.Description, delivery.RecipientCityId),
                    delivery.RecipientEmail);
            var deliveryMethod = deliveryType != null
                    ? new DeliveryMethod(
                        new DeliveryMethodType(deliveryType.Id, deliveryType.Title),
                        new DeliveryMethodCarrier(deliveryType.CarrierType.Id, deliveryType.CarrierType.Title)
                    )
                    : null;

            return new Delivery(
                deliveryMethod,
                recipient,
                delivery.Weight,
                delivery.Volume,
                delivery.DeclaredPrice,
                delivery.PayerType,
                delivery.PaymentMethod,
                delivery.CargoType,
                delivery.ServiceType,
                delivery.CashOnDelivery,
                null,
                delivery.CargoInvoice,
                delivery.ChangedAt);
        }
    }
}
