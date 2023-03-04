using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Data.Models;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Deliveries.Inner;
using LegacySql.Domain.Departments;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ClientOrderRepository : ILegacyClientOrderRepository
    {
        private readonly LegacyDbContext _db;
        private readonly LegacyDbConnection _connection;
        private readonly AppDbContext _mapDb;
        private readonly IProductMappingResolver _productMappingResolver;
        private readonly int _notFullMappingFilterPortion;
        private readonly int _orderPortion = 1000;
        private readonly IEnumerable<DepartmentEF> _departments;
        private readonly string _baseQuery;

        public ClientOrderRepository(
            LegacyDbContext db,
            AppDbContext mapDb,
            int notFullMappingFilterPortion,
            IProductMappingResolver productMappingResolver,
            LegacyDbConnection connection)
        {
            _db = db;
            _mapDb = mapDb;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
            _departments = GetDepartments();
            _productMappingResolver = productMappingResolver;
            _connection = connection;
            _baseQuery = $@"select dbo.GetOrderTag(rn.НомерПН) as isCashless, rn.*, cd.doc2ID, bill.оплачен from dbo.[рн] rn 
                                              left join [dbo].[connected_documents] cd on type1=1 and type2=10 and doc1ID in (rn.НомерПН)
                                              left join [dbo].[Счет] bill on bill.Код_счета = cd.doc2ID
                                              where rn.[Дата] > '2022-05-01' and (rn.[Paid]=0 and rn.[колонка] <> 8 or 
                                              (SELECT count(*) FROM [dbo].[connected_documents] 
                                              where type1=1 and type2=10 and doc1ID = rn.НомерПН) > 0 and bill.оплачен = 0)";
        }

        private IEnumerable<DepartmentEF> GetDepartments()
        {
            return _db.Departments.ToList();
        }

        private async IAsyncEnumerable<ClientOrder> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var clientOrders = await _db.ClientOrders
                .FromSqlRaw(_baseQuery)
                .Include(o => o.Client)
                .Where(o => o.Client.Department != 2 && !o.Client.IsSupplier)
#if DEBUG
                    .Take(1000)
#endif
                    .GroupBy(e=>e.Id)
                    .Select(group=>group.First())
                    .ToListAsync(cancellationToken);

            await foreach (var clientOrder in MapToDomain(clientOrders, cancellationToken))
            {
                yield return clientOrder;
            }
        }

        public async IAsyncEnumerable<ClientOrder> GetChangedClientOrdersAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (changedAt.HasValue)
            {
                var changedOrderItemsClientOrderIds = (await GetChangedClientItemsClientOrderIds(changedAt.Value, cancellationToken)).ToList();
                var changedBillsClientOrderIds = (await GetChangedBillsClientOrderIds(changedAt.Value, cancellationToken)).ToList();
                var changeDeliveryClientOrdersIds = (await GetChangedDeliveriesClientOrderIds(changedAt.Value, cancellationToken)).ToList();

                var changedClientOrderIds = new List<int>(changedOrderItemsClientOrderIds);
                changedClientOrderIds.AddRange(changedBillsClientOrderIds);
                changedClientOrderIds.AddRange(changeDeliveryClientOrdersIds);
                changedClientOrderIds.Distinct();

                List<ClientOrderEF> clientOrdersEfByDate = new List<ClientOrderEF>();
                string queryString;
                if (changedClientOrderIds.Any())
                {
                    var count = changedClientOrderIds.Count;
                    var cycleLimitation = (double)count / _notFullMappingFilterPortion;
                    for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
                    {
                        var portion = string.Join(",", changedClientOrderIds
                            .Skip(i * _notFullMappingFilterPortion)
                            .Take(_notFullMappingFilterPortion));

                        queryString = new StringBuilder(_baseQuery).Append($@" and rn.[НомерПН] in ({portion})").ToString();

                        clientOrdersEfByDate.AddRange((await _db.ClientOrders
                            .FromSqlRaw(queryString)
                            .Include(o => o.Client)
                            .Where(o => o.Client.Department != 2 && !o.Client.IsSupplier)
                            .ToListAsync(cancellationToken))
                            .GroupBy(e => e.Id)
                            .Select(group => group.First())
                            );
                    }
                }

                queryString = new StringBuilder(_baseQuery).Append($@" and (rn.[modified_at] > '{changedAt:yyyy.MM.dd HH:mm:ss.fff}' 
                                          or rn.[DataSozd] > '{changedAt:yyyy.MM.dd HH:mm:ss.fff}')").ToString();

                clientOrdersEfByDate.AddRange(await _db.ClientOrders
                    .FromSqlRaw(queryString)
                    .Include(o => o.Client)
                    .Where(o => o.Client.Department != 2 && !o.Client.IsSupplier)
                    .GroupBy(e => e.Id)
                    .Select(group => group.First())
#if DEBUG
                        .Take(10)
#endif
                        .ToListAsync(cancellationToken));


                await foreach (var clientOrder in MapToDomain(clientOrdersEfByDate.Distinct(), cancellationToken))
                {
                    yield return clientOrder;
                }

                await foreach (var clientOrder in GetByNotFullMapping(notFullMappingIds.ToList(), cancellationToken))
                {
                    yield return clientOrder;
                }
            }
            else
            {
                await foreach (var clientOrder in GetAllAsync(cancellationToken))
                {
                    yield return clientOrder;
                }
            }            
        }

        public async Task<IEnumerable<ClientOrder>> GetClientOrdersWithNotEndedWarrantyAsync(CancellationToken cancellationToken)
        {
            var connection = _db.Database.GetDbConnection();
            connection.Open();
            await using var command = connection.CreateCommand();
            command.CommandText =
                $"select count(*) as quantity from dbo.[рн] where [ф] = 1 and DATEADD(mm, orderItem.[warranty], rn.[Дата]) > CURRENT_TIMESTAMP";
            var result = command.ExecuteScalar().ToString();
            double.TryParse(result, out var ordersCount);

            var clientOrdersEf = new List<ClientOrderEF>();
            var cycleLimitation = Math.Ceiling(ordersCount / _orderPortion);
            for (var i = 0; i < cycleLimitation; i++)
            {
                var clientOrdersEfPortion = await _db.ClientOrders
                    .FromSqlRaw(
                        $@"select dbo.GetOrderTag(rn.НомерПН) as isCashless, rn.*, orderItem.warranty 
                        from dbo.[рн] rn inner join dbo.[Расход] as orderItem on rn.[НомерПН] = orderItem.[НомерПН] 
                        where [ф] = 1 and DATEADD(mm, orderItem.[warranty], rn.[Дата]) > CURRENT_TIMESTAMP 
                        order by[НомерПН] desc OFFSET {0 * _orderPortion} rows fetch next {_orderPortion} rows only")
                    .Include(o => o.Client)
#if DEBUG
                    .Take(1000)
#endif
                    .ToListAsync(cancellationToken);

                clientOrdersEf.AddRange(clientOrdersEfPortion);
            }

            var clientOrders = clientOrdersEf.GroupBy(e => e.Id).Select(e => e.First())
                .Select(async o => await MapToDomain(o, cancellationToken)).Select(o => o.Result);

            await connection.CloseAsync();

            return clientOrders;
        }

        private async Task<ClientOrder> MapToDomain(ClientOrderEF clientOrderEf, CancellationToken cancellationToken)
        {
            var clientOrderMap = await _mapDb.ClientOrderMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == clientOrderEf.Id, cancellationToken: cancellationToken);
            var clientMap = await _mapDb.ClientMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == clientOrderEf.ClientId, cancellationToken: cancellationToken);
            var warehouseMap = clientOrderEf.WarehouseId.HasValue ?
                await _mapDb.WarehouseMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == clientOrderEf.WarehouseId, cancellationToken: cancellationToken)
                : null;

            EmployeeMapEF employeeMap = null;
            var legacyEmployee = await _db.Employees.FirstOrDefaultAsync(e => e.NickName == clientOrderEf.Manager, cancellationToken: cancellationToken);
            if (legacyEmployee != null)
            {
                employeeMap = await _mapDb.EmployeeMaps.AsNoTracking()
                    .FirstOrDefaultAsync(em => em.LegacyId == legacyEmployee.Id, cancellationToken: cancellationToken);
            }

            var (billNumber, billFirmId, billChangetAt) = await GetBill(clientOrderEf.Id, cancellationToken);

            var orderItems = (await GetOrderItems(clientOrderEf, cancellationToken)).ToList();
            if (!orderItems.Any() && billNumber.HasValue)
            {
                orderItems = (await GetOrderItemsByBill(billNumber.Value, cancellationToken)).ToList();
            }
            var orderItemsChangedAt = orderItems.Max(e => e.ChangedAt);

            Delivery delivery = await GetDelivery(clientOrderEf, cancellationToken); 

            var changedTimeList = new List<DateTime?>
            {
                clientOrderEf.Date, clientOrderEf.ChangedAt, orderItemsChangedAt
            };
            if (billChangetAt.HasValue)
            {
                changedTimeList.Add(billChangetAt.Value);
            }
            if (delivery?.ChangedAt != null)
            {
                changedTimeList.Add(delivery.ChangedAt.Value);
            }
            var orderChangedAt = changedTimeList.Max();

            var clientOrderDto = new ClientOrder(
                id: new IdMap(clientOrderEf.Id, clientOrderMap?.ErpGuid),
                date: clientOrderEf.Date,
                clientId: new IdMap(clientOrderEf.ClientId, clientMap?.ErpGuid),
                comments: clientOrderEf.Comments,
                hasMap: clientOrderMap != null,
                paymentType: clientOrderEf.IsCashless ? PaymentTypes.Cashless : PaymentTypes.Cash,
                marketplaceNumber: clientOrderEf.MarketplaceNumber,
                source: GetSource(clientOrderEf, cancellationToken),
                delivery: delivery,
                isExecuted: clientOrderEf.IsExecuted,
                changedAt: orderChangedAt,
                isPaid: clientOrderEf.IsPaid,
                warehouseId: clientOrderEf.WarehouseId.HasValue ? new IdMap(clientOrderEf.WarehouseId.Value, warehouseMap?.ErpGuid) : null,
                managerId: legacyEmployee != null ? new IdMap(legacyEmployee.Id, employeeMap?.ErpGuid) : null,
                quantity: (!clientOrderEf.Quantity.HasValue || clientOrderEf.Quantity == 0) && !orderItems.Any()
                ? 0 : clientOrderEf.Quantity.HasValue && clientOrderEf.Quantity > 0
                ? clientOrderEf.Quantity.Value : orderItems.Sum(e => (int)e.Quantity),
                amount: (!clientOrderEf.Amount.HasValue || clientOrderEf.Amount == 0) && !orderItems.Any()
                ? 0 : clientOrderEf.Amount.HasValue && clientOrderEf.Amount > 0
                ? clientOrderEf.Amount.Value : orderItems.Sum(e => (double)e.PriceUAH),
                recipientOKPO: await GetOkpo(clientOrderEf.Id, cancellationToken),
                clientOrderEf.PaymentDate,
                items: orderItems,
                billNumber: billNumber,
                firmSqlId: billFirmId
            );

            return clientOrderDto;
        }

        private async Task<string> GetOkpo(int id, CancellationToken cancellationToken)
        {
            var documents = await _db.ConnectedDocuments
                .Where(d => d.Type1 == 1 && d.Type2 == 10 && d.Doc1Id == id)
                .ToListAsync(cancellationToken);

            if (!documents.Any())
            {
                return null;
            }

            var sqlQuery = @"select okpo from [dbo].[OOO]
                            where ID in 
                            (select OOO from [dbo].[Счет]
                            where Код_счета in (@checkId))";

            using (var connection = _connection.Connection)
            {
                var result = await connection.QueryAsync<string>(sqlQuery, new
                {
                    checkId = documents.First().Doc2Id
                });
                return result.FirstOrDefault();
            }
        }

        private async IAsyncEnumerable<ClientOrder> MapToDomain(IEnumerable<ClientOrderEF> clientOrdersEf, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var p in clientOrdersEf)
            {
                var product = await MapToDomain(p, cancellationToken);
                if (product != null)
                {
                    yield return product;
                }
            }
        }

        private async Task<IEnumerable<ClientOrderItem>> GetOrderItems(ClientOrderEF clientOrderEf, CancellationToken cancellationToken)
        {
            var orderItems = (await _db.ClientOrderItems
                .Include(e => e.Nomenclature)
                    .Where(e => e.ClientOrderId == clientOrderEf.Id)
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
                        e.Warranty,
                        SerialNumbers = e.SerialNumbers.Select(sn => sn.SerialNumber),
                        e.ChangedAt
                    })
                    .ToListAsync(cancellationToken));

            var clientOrderItems = orderItems.Select(async item =>
                new ClientOrderItem(
                item.NomenclatureId.HasValue
                    ? await GetProductMapping(item.NomenclatureId.Value,
                        item.Nomenclature?.NonCashProductId,
                        cancellationToken)
                    : null,
                item.Quantity ?? 0,
                item.Price,
                item.PriceUAH,
                item.Warranty ?? 0,
                item.SerialNumbers,
                item.ChangedAt)
                ).Select(e => e.Result);

            return clientOrderItems.Where(e => e.NomenclatureId?.InnerId != 0 && e.NomenclatureId != null);
        }

        private async Task<IEnumerable<int>> GetChangedDeliveriesClientOrderIds(DateTime changetAt, CancellationToken cancellationToken)
        {
            var changedDeliveries = await _db.Deliveries
                .Where(e => e.ChangedAt > changetAt)
                .Select(e=> e.Id)
                .ToListAsync(cancellationToken);
            
            var connectedDocuments = await _db.ConnectedDocuments
                .Where(e => (changedDeliveries.Contains(e.Doc2Id) && e.Type1 == 1 && e.Type2 == 16)
                                          || (changedDeliveries.Contains(e.Doc1Id) && e.Type1 == 16 && e.Type2 == 1))
                .ToListAsync(cancellationToken);

            if (!connectedDocuments.Any())
            {
                return new List<int>();
            }

            return connectedDocuments.Select(e => e.Type1 == 1 ? e.Doc1Id : e.Doc2Id);
        }

        private async Task<Delivery> GetDelivery(ClientOrderEF order, CancellationToken cancellationToken)
        {
            var connectedDocuments = await _db.ConnectedDocuments
                .FirstOrDefaultAsync(e => (e.Doc1Id == order.Id && e.Type1 == 1 && e.Type2 == 16)
                                          || (e.Doc2Id == order.Id && e.Type1 == 16 && e.Type2 == 1), cancellationToken);

            if (connectedDocuments == null)
            {
                return null;
            }

            var delivery = await _db.Deliveries.FirstOrDefaultAsync(
                e => e.Id == (connectedDocuments.Type1 == 1 ? connectedDocuments.Doc2Id : connectedDocuments.Doc1Id), cancellationToken);

            if(delivery == null)
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

            MarketplaceDeliveryEF marketplaceDelivery = null;
            if (!string.IsNullOrEmpty(order.MarketplaceNumber))
            {

                var isParseSuccessful = int.TryParse(order.MarketplaceNumber, out var marketplaceNumber);
                if (isParseSuccessful)
                {
                    marketplaceDelivery = await _db.MarketplaceDeliveries
                        .FirstOrDefaultAsync(m => m.MarketplaceNumber == marketplaceNumber, cancellationToken);
                }
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
                marketplaceDelivery == null
                    ? null
                    : new DeliveryWarehouse(marketplaceDelivery.WarehouseNumber, marketplaceDelivery.WarehouseId), 
                delivery.CargoInvoice,
                delivery.ChangedAt);

        }

        private ClientOrderSource GetSource(ClientOrderEF clientOrderEf, CancellationToken cancellationToken)
        {
            if (clientOrderEf.Manager == "inet")
            {
                return ClientOrderSource.CreateOptDclinkSource();
            }

            var department = _departments.FirstOrDefault(d => d.Id == clientOrderEf.Client.Department);
            return department == null ? null : new ClientOrderSource(department.Title);
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            return new IdMap(productMainSqlId, productErpGuid);
        }

        private async Task<IEnumerable<int>> GetChangedClientItemsClientOrderIds(DateTime changedAt,
            CancellationToken cancellationToken)
        {
            return await _db.ClientOrderItems
                .Where(p => p.ChangedAt.HasValue && p.ChangedAt > changedAt)
                .GroupBy(p => p.ClientOrderId)
                .Select(p => p.Key).ToListAsync(cancellationToken: cancellationToken);
        }

        private async Task<IEnumerable<int>> GetChangedBillsClientOrderIds(DateTime changetAt, CancellationToken cancellationToken)
        {
            var connectedDocuments = await _db.ConnectedDocuments
                .Where(e => e.Date.HasValue && e.Date > changetAt && ((e.Type1 == 1 && e.Type2 == 10)
                                          || (e.Type1 == 10 && e.Type2 == 1)))
                .ToListAsync(cancellationToken);

            if (!connectedDocuments.Any())
            {
                return new LinkedList<int>();
            }

            return connectedDocuments.Select(e=> e.Type1 == 1 ? e.Doc1Id : e.Doc2Id);
        }

        private async IAsyncEnumerable<ClientOrder> GetByNotFullMapping(List<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
                yield break;
            }

            var notFullMappingCount = notFullMappingIds.Count();
            var cycleLimitation = (double)notFullMappingCount / _notFullMappingFilterPortion;
            var clientOrdersEf = new List<ClientOrderEF>();
            for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
            {
                var notFullMappingIdsPortion = string.Join(",", notFullMappingIds
                    .Skip(i * _notFullMappingFilterPortion)
                    .Take(_notFullMappingFilterPortion));
                clientOrdersEf.AddRange(await _db.ClientOrders
                    .FromSqlRaw($"select dbo.GetOrderTag(rn.НомерПН) as isCashless, rn.* from dbo.[рн] rn where [НомерПН] IN ({notFullMappingIdsPortion})")
                    .Include(o => o.Client)
                    .ToListAsync(cancellationToken));                
            }

            await foreach (var clientOrder in MapToDomain(clientOrdersEf, cancellationToken))
            {
                yield return clientOrder;
            }
        }

        public async Task<ClientOrder> GetClientOrderWithAllFiltersAsync(int id, CancellationToken cancellationToken)
        {
            var clientOrderEf = await _db.ClientOrders
                .FromSqlRaw(@$"{_baseQuery} and [НомерПН] = {id}")
                .Include(o => o.Client)
                .Where(o => o.Client.Department != 2 && !o.Client.IsSupplier)
                .GroupBy(e => e.Id)
                .Select(group => group.First())
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (clientOrderEf == null)
            {
                return null;
            }

            return await MapToDomain(clientOrderEf, cancellationToken);
        }

        public async Task<ClientOrder> GetClientOrderAsync(int id, CancellationToken cancellationToken)
        {
            var clientOrderEf = await _db.ClientOrders
                .FromSqlRaw(@$"select dbo.GetOrderTag([НомерПН]) as isCashless, * from dbo.[РН] 
                              where [НомерПН] = {id}")
                .Include(o => o.Client)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (clientOrderEf == null)
            {
                return null;
            }

            return await MapToDomain(clientOrderEf, cancellationToken);
        }

        private async Task<(int? billNumber, int? firmId, DateTime? changetAt)> GetBill(int orderId, CancellationToken cancellationToken)
        {
            var connectedDocuments = await _db.ConnectedDocuments
                .FirstOrDefaultAsync(e => (e.Doc1Id == orderId && e.Type1 == 1 && e.Type2 == 10)
                                          || (e.Doc2Id == orderId && e.Type1 == 10 && e.Type2 == 1), cancellationToken);

            if (connectedDocuments == null)
            {
                return (null, null, null);
            }

            var billId = connectedDocuments.Type1 == 1 ? connectedDocuments.Doc2Id : connectedDocuments.Doc1Id;
            var bill = await _db.Bills.FirstOrDefaultAsync(e => e.Id == billId, cancellationToken);
            return (bill?.Number, bill?.FirmId, bill.Date);
        }

        private async Task<IEnumerable<ClientOrderItem>> GetOrderItemsByBill(int billSqlId, CancellationToken cancellationToken)
        {
            var orderItems = await _db.BillItems
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
                })
                .ToListAsync(cancellationToken);

            var bill = await _db.Bills.FirstOrDefaultAsync(e => e.Id == billSqlId);

            if(!orderItems.Any() && (bill == null || bill.Issued != true) )
            {
                return new List<ClientOrderItem>();
            }

            var clientOrderItems = orderItems.Select(async item =>
                new ClientOrderItem(
                await GetProductMapping(item.NomenclatureId,
                        item.Nomenclature?.NonCashProductId,
                        cancellationToken),
                item.Quantity ?? 0,
                item.Price,
                item.PriceUAH,
                item.Warranty ?? 0,
                new List<string>(),
                null)
            ).Select(e => e.Result);

            return clientOrderItems.Where(e=>e.NomenclatureId?.InnerId != 0);
        }
    }
}