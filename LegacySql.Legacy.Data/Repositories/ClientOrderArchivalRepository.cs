using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ClientOrderArchivalRepository : ILegacyClientOrderArchivalRepository
    {
        private readonly LegacyDbContext _db;
        private readonly LegacyDbConnection _connection;
        private readonly AppDbContext _mapDb;
        private readonly int _orderPortion = 100000;
        private readonly IEnumerable<DepartmentEF> _departments;
        private readonly IProductMappingResolver _productMappingResolver;

        public ClientOrderArchivalRepository(LegacyDbContext db, AppDbContext mapDb, IProductMappingResolver productMappingResolver, LegacyDbConnection connection)
        {
            _db = db;
            _mapDb = mapDb;
            _productMappingResolver = productMappingResolver;
            _departments = GetDepartments();
            _connection = connection;
        }

        private IEnumerable<DepartmentEF> GetDepartments()
        {
            return _db.Departments.ToList();
        }

        public async Task<IEnumerable<ClientOrder>> GetClientOrdersAsync(CancellationToken cancellationToken)
        {
            var connection = _db.Database.GetDbConnection();

            connection.Open();

            await using var command = connection.CreateCommand();
            command.CommandText =
                $"select count(*) as quantity from dbo.[РН_Arch] rn inner join dbo.[Расход_Arch] as orderItem on rn.[НомерПН] = orderItem.[НомерПН] where DATEADD(mm, orderItem.[warranty], rn.[Дата]) > '{DateTime.Today:yyyy.MM.dd}'";
            var result = await command.ExecuteScalarAsync(cancellationToken);
            double.TryParse(result.ToString(), out var ordersCount);

            var clientOrdersEf = new List<ClientOrderArchivalEF>();
            var cycleLimitation = Math.Ceiling(ordersCount / _orderPortion);
            for (var i = 0; i < cycleLimitation; i++)
            {
                var clientOrdersEfPortion = await _db.ClientOrdersArchival
                    .FromSqlRaw(
                        $"select dbo.GetOrderTag(rn.НомерПН) as isCashless, rn.*, orderItem.warranty from dbo.[РН_Arch] rn inner join dbo.[Расход_Arch] as orderItem on rn.[НомерПН] = orderItem.[НомерПН] where DATEADD(mm, orderItem.[warranty], rn.[Дата]) > '{DateTime.Today:yyyy.MM.dd}' order by [НомерПН] asc OFFSET {0 * _orderPortion} rows fetch next {_orderPortion} rows only")
                    .Include(o => o.Client)
#if DEBUG
                    .Take(1000)
#endif
                    .ToListAsync(cancellationToken);

                clientOrdersEf.AddRange(clientOrdersEfPortion);
            }

            var clientOrders = clientOrdersEf.GroupBy(e => e.Id).Select(e => e.First()).Select(async o => await MapToDomain(o, cancellationToken))
                .Select(o => o.Result);

            await connection.CloseAsync();

            return clientOrders;
        }

        private async Task<ClientOrder> MapToDomain(ClientOrderArchivalEF clientOrderEf, CancellationToken cancellationToken)
        {
            var orderItems = await GetOrderItems(clientOrderEf, cancellationToken);
            var clientOrderMap = await _mapDb.ClientOrderMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == clientOrderEf.Id, cancellationToken: cancellationToken);
            var clientMap = await _mapDb.ClientMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == clientOrderEf.ClientId, cancellationToken: cancellationToken);
            var warehouseMap = clientOrderEf.WarehouseId.HasValue ?
                await _mapDb.WarehouseMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == clientOrderEf.WarehouseId, cancellationToken: cancellationToken) :
                null;

            EmployeeMapEF employeeMap = null;
            var legacyEmployee = await _db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.NickName == clientOrderEf.Manager, cancellationToken: cancellationToken);
            if (legacyEmployee != null)
            {
                employeeMap = await _mapDb.EmployeeMaps.AsNoTracking()
                    .FirstOrDefaultAsync(em => em.LegacyId == legacyEmployee.Id, cancellationToken: cancellationToken);
            }

            var (billNumber, billFirmId) = await GetBill(clientOrderEf.Id, cancellationToken);

            var clientOrderDto = new ClientOrder(
                id: new IdMap(clientOrderEf.Id, clientOrderMap?.ErpGuid),
                date: clientOrderEf.Date,
                clientId: new IdMap(clientOrderEf.ClientId, clientMap?.ErpGuid),
                comments: clientOrderEf.Comments,
                hasMap: clientOrderMap != null,
                paymentType: clientOrderEf.IsCashless ? PaymentTypes.Cashless : PaymentTypes.Cash,
                marketplaceNumber: clientOrderEf.MarketplaceNumber,
                source: GetSource(clientOrderEf, cancellationToken),
                delivery: await GetDelivery(clientOrderEf, cancellationToken),
                isExecuted: clientOrderEf.IsExecuted,
                changedAt: clientOrderEf.ChangedAt,
                isPaid: clientOrderEf.IsPaid,
                warehouseId: clientOrderEf.WarehouseId.HasValue ? new IdMap(clientOrderEf.WarehouseId.Value, warehouseMap?.ErpGuid) : null,
                managerId: legacyEmployee != null ? new IdMap(legacyEmployee.Id, employeeMap?.ErpGuid) : null,
                items: orderItems,
                quantity:clientOrderEf.Quantity ?? 0,
                amount:clientOrderEf.Amount ?? 0,
                paymentDate: clientOrderEf.PaymentDate,
                recipientOKPO: await GetOkpo(clientOrderEf.Id, cancellationToken),
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

            if (!documents.Any() || documents.Count() > 1)
            {
                return null;
            }

            var sqlQuery = @"select okpo from [skl2008_tqm].[dbo].[OOO]
                            where ID in 
                            (select OOO from [skl2008_tqm].[dbo].[Счет]
                            where Код_счета in (@checkId))";

            var result = await _connection.Connection.QueryAsync<string>(sqlQuery, new
            {
                checkId = documents.First().Doc2Id
            });

            return result.FirstOrDefault();
        }

        private async Task<IEnumerable<ClientOrderItem>> GetOrderItems(ClientOrderArchivalEF clientOrderEf, CancellationToken cancellationToken)
        {
            var orderItemsEf = await _db.ClientOrderItemsArchival.Include(e => e.SerialNumbers)
                .Include(e => e.Nomenclature)
                .Where(e => e.ClientOrderId == clientOrderEf.Id)
                .ToListAsync(cancellationToken);
            var orderItems = new List<ClientOrderItem>();
            foreach (var item in orderItemsEf)
            {
                orderItems.Add(new ClientOrderItem(
                    item.NomenclatureId.HasValue
                        ? await GetProductMapping(item.NomenclatureId.Value,
                                                  item.Nomenclature?.NonCashProductId,
                                                  cancellationToken)
                        : null,
                    item.Quantity ?? 0,
                    item.Price,
                    item.PriceUAH,
                    item.Warranty ?? (short)0,
                    item.SerialNumbers.Select(n => n.SerialNumber),
                    item.ChangedAt));
            }

            return orderItems;
        }
        public async Task<Delivery> GetDelivery(ClientOrderArchivalEF order, CancellationToken cancellationToken)
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

            if (delivery == null)
            {
                return null;
            }

            NewPostCityEF newPostCity = null;
            DeliveryTypeEF deliveryType = null;

            if (delivery.RecipientCityId.HasValue)
            {
                newPostCity = await _db.NewPostCities.FirstOrDefaultAsync(
                    e => e.CityRef == delivery.RecipientCityId,
                    cancellationToken);
            }

            if (delivery.TypeId.HasValue)
            {
                deliveryType = await _db.DeliveryTypes.Include(e => e.CarrierType)
                    .FirstOrDefaultAsync(e => e.Id == delivery.TypeId, cancellationToken);
            }

            MarketplaceDeliveryEF marketplaceDelivery = null;
            if (!string.IsNullOrEmpty(order.MarketplaceNumber))
            {
                marketplaceDelivery = await _db.MarketplaceDeliveries
                    .FirstOrDefaultAsync(m => m.WarehouseNumber == order.MarketplaceNumber, cancellationToken);
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

        public ClientOrderSource GetSource(ClientOrderArchivalEF clientOrderEf, CancellationToken cancellationToken)
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

        private async Task<(int? billNumber, int? firmId)> GetBill(int orderId, CancellationToken cancellationToken)
        {
            var connectedDocuments = await _db.ConnectedDocuments
                .FirstOrDefaultAsync(e => (e.Doc1Id == orderId && e.Type1 == 1 && e.Type2 == 10)
                                          || (e.Doc2Id == orderId && e.Type1 == 10 && e.Type2 == 1), cancellationToken);

            if (connectedDocuments == null)
            {
                return (null, null);
            }

            var billId = connectedDocuments.Type1 == 1 ? connectedDocuments.Doc2Id : connectedDocuments.Doc1Id;
            var bill = await _db.Bills.FirstOrDefaultAsync(e => e.Id == billId, cancellationToken);
            return (bill?.Number, bill?.FirmId);
        }
    }
}
