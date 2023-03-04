using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Clients;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Warehouses;
using LegacySql.Legacy.Data.Models;
using MessageBus.ClientOrder.Import;

namespace LegacySql.Consumers.Commands.ClientOrders
{
    public class ErpClientOrderSaver
    {
        private readonly IDbConnection _db;
        private readonly IErpChangedRepository _erpChangedRepository;
        private readonly IClientOrderMapRepository _clientOrderMapRepository;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private ExternalMap _clientMapping;
        private IEnumerable<ExternalMap> _clientOrderMappings = new List<ExternalMap>();
        private Dictionary<Guid, int> _warehouseMappings = new Dictionary<Guid, int>();
        private ErpClientOrderDto _order;

        public ErpClientOrderSaver(
            IDbConnection db,
            IErpChangedRepository erpChangedRepository,
            IClientOrderMapRepository clientOrderMapRepository,
            IClientMapRepository clientMapRepository,
            IProductMapRepository productMapRepository,
            IWarehouseMapRepository warehouseMapRepository)
        {
            _db = db;
            _erpChangedRepository = erpChangedRepository;
            _clientOrderMapRepository = clientOrderMapRepository;
            _clientMapRepository = clientMapRepository;
            _productMapRepository = productMapRepository;
            _warehouseMapRepository = warehouseMapRepository;
        }

        public void InitErpObject(ErpClientOrderDto order, IEnumerable<ExternalMap> clientOrderMapping)
        {
            if (!clientOrderMapping.Any() && !order.Items.Any())
            {
                throw new DataException("Нельзя создать заказ без товаров");
            }

            _order = order;
            this._clientOrderMappings = clientOrderMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            _clientMapping = await _clientMapRepository.GetByErpAsync(_order.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{_order.ClientId} не найден\n");
            }

            if (_order.Items.Any())
            {
                foreach (var orderItem in _order.Items)
                {
                    var productMapping = await _productMapRepository.GetByErpAsync(orderItem.NomenclatureId);
                    if (productMapping == null)
                    {
                        why.Append($"Маппинг продукта id: {orderItem.NomenclatureId} не найден\n");
                    }
                }
            }

            if (_order.Items.Any())
            {
                foreach (var item in _order.Items.GroupBy(e => e.WarehouseId))
                {
                    if (item.Key.HasValue)
                    {
                        var warehouseMapping = await _warehouseMapRepository.GetByErpAsync(item.Key.Value);
                        if (warehouseMapping == null)
                        {
                            why.Append($"Маппинг склада id:{item.Key.Value} не найден\n");
                            continue;
                        }

                        if (!_warehouseMappings.ContainsKey(item.Key.Value))
                        {
                            _warehouseMappings.Add(item.Key.Value, warehouseMapping.LegacyId);
                        }
                    }
                }
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task SaveErpObject()
        {
            if (!_clientOrderMappings.Any())
            {
                await Create();
            }
            else
            {
                await Update();
            }
        }

        private async Task Create()
        {
            var newClientOrdersIds = new List<int>();
            var newClientOrdersMaps = new List<int>();
            _db.Open();
            using var transaction = _db.BeginTransaction();
            var clientOrderItemsByWarehouse = _order.Items.Where(e=>e.WarehouseId.HasValue).GroupBy(e => e.WarehouseId);
            try
            {
                foreach (var itemsGroup in clientOrderItemsByWarehouse)
                {
                    var newOrderId = await CreateOrder(_warehouseMappings[itemsGroup.Key.Value], transaction);
                    await InsertOrderItems(itemsGroup, transaction, newOrderId);
                    
                    newClientOrdersMaps.Add(newOrderId);
                    newClientOrdersIds.Add(newOrderId);
                }

                var clientOrderItemsWithoutWarehouse = _order.Items.Where(e => !e.WarehouseId.HasValue).GroupBy(e => e.WarehouseId).FirstOrDefault();
                if (clientOrderItemsWithoutWarehouse != null)
                {
                    int orderId;
                    if (!newClientOrdersIds.Any())
                    {
                        var principalWarehouseSqlIdSqlQuery = @"select [sklad_ID][sklad_desc] from [dbo].[Склады] 
                                                              where [sklad_desc]=@Description";
                        var principalWarehouseSqlId = await _db.QueryFirstOrDefaultAsync<int?>(principalWarehouseSqlIdSqlQuery, new
                        {
                            Description = "Основной"
                        });
                        var principalWarehouseMapping = principalWarehouseSqlId.HasValue 
                            ? await _warehouseMapRepository.GetByLegacyAsync(principalWarehouseSqlId.Value) ?? await _warehouseMapRepository.GetFirstMappingAsync() 
                            : await _warehouseMapRepository.GetFirstMappingAsync();
                        orderId = await CreateOrder(principalWarehouseMapping.LegacyId, transaction);
                    }
                    else
                    {
                        orderId = newClientOrdersIds.Min();
                    }
                    await InsertOrderItems(clientOrderItemsWithoutWarehouse, transaction, orderId);
                }

                transaction.Commit();

                foreach (var newOrderId in newClientOrdersMaps)
                {
                    await _clientOrderMapRepository.SaveAsync(new ExternalMap(Guid.NewGuid(), newOrderId, _order.Id));
                }
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                _db.Close();
            }
        }

        private async Task<int> CreateOrder(int? warehouseId, IDbTransaction transaction)
        {
            var insertOrderQuery = new StringBuilder();
            insertOrderQuery.Append(@"insert into [dbo].[РН] ([Дата],[klientID],[Отдел],[Сумма],[Кол]");
            if (_order.State == ClientOrderStates.WaitAgreement
                || _order.State == ClientOrderStates.ToEnsure
                || _order.State == ClientOrderStates.ReadyToEnsure
                || _order.State == ClientOrderStates.ToShipment
                || _order.State == ClientOrderStates.WaitEnsure)
            {
                insertOrderQuery.Append(
                    ",[д2]) values (@Date,@ClientId,@WarehouseId,@Amount,@Quantity,1);select cast(SCOPE_IDENTITY() as int)");
            }
            else if (_order.State == ClientOrderStates.InProcessOfShipment
                     || _order.State == ClientOrderStates.AfterShipment
                     || _order.State == ClientOrderStates.ReadyToClose
                     || _order.State == ClientOrderStates.Close)
            {
                insertOrderQuery.Append(
                    ",[ф]) values (@Date,@ClientId,@WarehouseId,@Amount,@Quantity,1);select cast(SCOPE_IDENTITY() as int)");
            }
            else if (_order.State == ClientOrderStates.ReadyToShipment)
            {
                insertOrderQuery.Append(
                    ",[разрешил]) values (@Date,@ClientId,@WarehouseId,@Amount,@Quantity,1);select cast(SCOPE_IDENTITY() as int)");
            }
            else
            {
                insertOrderQuery.Append(
                    ") values (@Date,@ClientId,@WarehouseId,@Amount,@Quantity);select cast(SCOPE_IDENTITY() as int)");
            }

            var newOrderId = (await _db.QueryAsync<int>(insertOrderQuery.ToString(), new
            {
                _order.Date,
                ClientId = _clientMapping.LegacyId,
                WarehouseId = warehouseId,
                Amount = _order.Amount,
                Quantity = _order.Quantity
            }, transaction)).FirstOrDefault();

            return newOrderId;
        }

        private async Task InsertOrderItems(IGrouping<Guid?, ErpClientOrderItemDto> itemsGroup, IDbTransaction transaction, int newOrderId)
        {
            var insertOrderItemQuery = @"insert into [dbo].[Расход] 
                                       ([НомерПН],[КодТовара],[Цена],[ЦенаГРН],[КолЗ],[warranty],[марка],[КодТипа])
                                       values (@OrderId,@ProductId,@Price,@PriceUah,@Quantity,@Warranty,@Brand,@ProductTypeId)";
            foreach (var orderItem in itemsGroup)
            {
                var productMapping = await _productMapRepository.GetByErpAsync(orderItem.NomenclatureId);
                var productInfo = await GetProductInfo(productMapping.LegacyId, transaction);
                await _db.ExecuteAsync(insertOrderItemQuery, new
                {
                    OrderId = newOrderId,
                    ProductId = productMapping.LegacyId,
                    Price = orderItem.Price,
                    PriceUah = orderItem.PriceUAH,
                    Quantity = orderItem.Quantity,
                    Warranty = orderItem.Warranty,
                    Brand = productInfo.Brand,
                    ProductTypeId = productInfo.ProductTypeId
                }, transaction);
            }
        }

        private async Task Update()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                foreach (var clientOrderMapping in _clientOrderMappings)
                {
                    await UpdateClientOrder(clientOrderMapping, transaction);
                    await UpdateClientOrderItems(clientOrderMapping, transaction);
                    await SaveInErpChange(clientOrderMapping, transaction);

                    if (_order.Status == ClientOrderStatuses.ToImplementation &&
                        _order.State == ClientOrderStates.ReadyToShipment)
                    {
                        await UpdateMarkVipolneno(clientOrderMapping.LegacyId, transaction);
                    }
                }

                var minOrderId = _clientOrderMappings.Min(w => w.LegacyId);
                await UpdateClientOrderItemsWithoutWarehouse(_clientOrderMappings.First(e=>e.LegacyId == minOrderId), transaction);

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                _db.Close();
            }
        }

        private async Task UpdateClientOrder(ExternalMap clientOrderMapping, IDbTransaction transaction)
        {
            var updateOrderQuery = new StringBuilder();
            updateOrderQuery.Append("update [dbo].[РН] set [Сумма]=@Amount,[Кол]=@Quantity");
            if (_order.State == ClientOrderStates.WaitAgreement
                || _order.State == ClientOrderStates.ToEnsure
                || _order.State == ClientOrderStates.ReadyToEnsure
                || _order.State == ClientOrderStates.ToShipment
                || _order.State == ClientOrderStates.WaitEnsure)
            {
                updateOrderQuery.Append(",[д2]=1,[ф]=0");
            }
            else if (_order.State == ClientOrderStates.InProcessOfShipment
                     || _order.State == ClientOrderStates.AfterShipment
                     || _order.State == ClientOrderStates.ReadyToClose
                     || _order.State == ClientOrderStates.Close)
            {
                updateOrderQuery.Append(",[ф]=1");
            }else if (_order.State == ClientOrderStates.ReadyToShipment)
            {
                updateOrderQuery.Append(",[разрешил]=1,[ф]=0");
            }

            updateOrderQuery.Append(" where [НомерПН]=@ClientOrderId");

            await _db.ExecuteAsync(updateOrderQuery.ToString(), new
            {
                ClientOrderId = clientOrderMapping.LegacyId,
                Amount = _order.Amount,
                Quantity = _order.Quantity
            }, transaction);
        }

        private async Task UpdateClientOrderItems(ExternalMap clientOrderMapping, IDbTransaction transaction)
        {
            var currentWarehouseMapping = await GetCurrentWarehouseMapping(clientOrderMapping, transaction);

            var orderProductsIds = await GetOrderProductsIds(clientOrderMapping, transaction);

            var orderItemsByWarehouse = _order.Items.Where(e => e.WarehouseId == currentWarehouseMapping.Key).ToList();
            var allOrderItems = _order.Items.Where(e => e.WarehouseId == currentWarehouseMapping.Key || !e.WarehouseId.HasValue).ToList();
            var productsMappings = (await _productMapRepository.GetRangeByErpIdsAsync(allOrderItems.Select(e => e.NomenclatureId))).ToList();
            foreach (var orderItem in orderItemsByWarehouse)
            {
                var productMapping = productsMappings.First(e => e.ExternalMapId == orderItem.NomenclatureId);
                var productInfo = await GetProductInfo(productMapping.LegacyId, transaction);
                if (orderProductsIds.Any(e => e == productMapping.LegacyId))
                {
                    await UpdateOrderItem(clientOrderMapping, transaction, productMapping, orderItem, productInfo);
                }
                else
                {
                    await InsertOrderItem(clientOrderMapping, transaction, productMapping, orderItem, productInfo);
                }
            }

            var productsIdsForDelete = orderProductsIds.Where(e => productsMappings.All(i => i.LegacyId != e));
            await DeleteOrderItems(clientOrderMapping, transaction, productsIdsForDelete);
        }

        private async Task UpdateClientOrderItemsWithoutWarehouse(ExternalMap clientOrderMapping, IDbTransaction transaction)
        {
            var currentWarehouseMapping = await GetCurrentWarehouseMapping(clientOrderMapping, transaction);

            var orderProductsIds = await GetOrderProductsIds(clientOrderMapping, transaction);

            var orderItemsWithoutWarehouse = _order.Items.Where(e => !e.WarehouseId.HasValue).ToList();
            var allOrderItems = _order.Items.Where(e => e.WarehouseId == currentWarehouseMapping.Key || !e.WarehouseId.HasValue).ToList();
            var productsMappings = (await _productMapRepository.GetRangeByErpIdsAsync(allOrderItems.Select(e => e.NomenclatureId))).ToList();
            foreach (var orderItem in orderItemsWithoutWarehouse)
            {
                var productMapping = productsMappings.First(e => e.ExternalMapId == orderItem.NomenclatureId);
                var productInfo = await GetProductInfo(productMapping.LegacyId, transaction);
                if (orderProductsIds.Any(e => e == productMapping.LegacyId))
                {
                    await UpdateOrderItem(clientOrderMapping, transaction, productMapping, orderItem, productInfo);
                }
                else
                {
                    await InsertOrderItem(clientOrderMapping, transaction, productMapping, orderItem, productInfo);
                }
            }

            var productsIdsForDelete = orderProductsIds.Where(e => productsMappings.All(i => i.LegacyId != e));
            await DeleteOrderItems(clientOrderMapping, transaction, productsIdsForDelete);

            await SaveInErpChange(clientOrderMapping, transaction);
        }

        private async Task<KeyValuePair<Guid, int>> GetCurrentWarehouseMapping(ExternalMap clientOrderMapping, IDbTransaction transaction)
        {
            var selectWarehouseIdQuery = @"select [Отдел] as WarehouseId FROM [dbo].[РН]
                                                 where [НомерПН]=@OrderId";
            var warehouseLegacyId = await _db.QueryFirstOrDefaultAsync<int>(selectWarehouseIdQuery, new
            {
                OrderId = clientOrderMapping.LegacyId
            }, transaction);
            var currentWarehouseMapping = _warehouseMappings.FirstOrDefault(e => e.Value == warehouseLegacyId);
            return currentWarehouseMapping;
        }

        private async Task<List<int>> GetOrderProductsIds(ExternalMap clientOrderMapping, IDbTransaction transaction)
        {
            var getOrderProductsQuery = @"select [КодТовара] from [dbo].[Расход]
                                        where [НомерПН]=@OrderId";

            var orderProductsIds = (await _db.QueryAsync<int>(getOrderProductsQuery, new
            {
                OrderId = clientOrderMapping.LegacyId
            }, transaction)).ToList();
            return orderProductsIds;
        }

        private async Task DeleteOrderItems(ExternalMap clientOrderMapping, IDbTransaction transaction,
            IEnumerable<int> productsIdsForDelete)
        {
            var updateItemQuantityQuery = @"update [dbo].[Расход] 
                                          set [КолЗ]=0
                                          where [НомерПН]=@OrderId and [КодТовара]=@ProductId";
            foreach (var productId in productsIdsForDelete)
            {
                await _db.ExecuteAsync(updateItemQuantityQuery, new
                {
                    OrderId = clientOrderMapping.LegacyId,
                    ProductId = productId
                }, transaction);
            }
        }

        private async Task InsertOrderItem(ExternalMap clientOrderMapping, IDbTransaction transaction,
            ExternalMap productMapping, ErpClientOrderItemDto orderItem, ProductInfo productInfo)
        {
            var insertOrderItemQuery = @"insert into [dbo].[Расход] 
                                               ([НомерПН],[КодТовара],[Цена],[ЦенаГРН],[КолЗ],[warranty],[марка],[КодТипа])
                                               values (@OrderId,@ProductId,@Price,@PriceUah,@Quantity,@Warranty,@Brand,@ProductTypeId)";
            await _db.ExecuteAsync(insertOrderItemQuery, new
            {
                OrderId = clientOrderMapping.LegacyId,
                ProductId = productMapping.LegacyId,
                Price = orderItem.Price,
                PriceUah = orderItem.PriceUAH,
                Quantity = orderItem.Quantity,
                Warranty = orderItem.Warranty,
                Brand = productInfo.Brand,
                ProductTypeId = productInfo.ProductTypeId
            }, transaction);
        }

        private async Task UpdateOrderItem(ExternalMap clientOrderMapping, IDbTransaction transaction,
            ExternalMap productMapping, ErpClientOrderItemDto orderItem, ProductInfo productInfo)
        {
            var updateOrderItemQuery = @"update [dbo].[Расход] 
                                               set [Цена]=@Price,
                                               [ЦенаГРН]=@PriceUah,
                                               [КолЗ]=@Quantity,
                                               [warranty]=@Warranty,
                                               [марка]=@Brand,
                                               [КодТипа]=@ProductTypeId
                                               where [НомерПН]=@OrderId and [КодТовара]=@ProductId";
            await _db.ExecuteAsync(updateOrderItemQuery, new
            {
                OrderId = clientOrderMapping.LegacyId,
                ProductId = productMapping.LegacyId,
                Price = orderItem.Price,
                PriceUah = orderItem.PriceUAH,
                Quantity = orderItem.Quantity,
                Warranty = orderItem.Warranty,
                Brand = productInfo.Brand,
                ProductTypeId = productInfo.ProductTypeId
            }, transaction);
        }

        
        private async Task SaveInErpChange(ExternalMap clientOrderMapping, IDbTransaction transaction)
        {
            var orderChangeDates = new List<DateTime>();
            var queryParams = new
            {
                ClientOrderId = clientOrderMapping.LegacyId
            };

            var selectOrderChangedDateQuery = @"select [modified_at] from [dbo].[РН]
                                       where [НомерПН]=@ClientOrderId";
            orderChangeDates.Add( await _db.QueryFirstOrDefaultAsync<DateTime>(selectOrderChangedDateQuery, queryParams, transaction));

            var selectOrderCreateDateQuery = @"select [DataSozd] from [dbo].[РН]
                                       where [НомерПН]=@ClientOrderId";
            orderChangeDates.Add(await _db.QueryFirstOrDefaultAsync<DateTime>(selectOrderCreateDateQuery, queryParams, transaction));

            var selectOrderItemsChangedDateQuery = @"select [modified_at] from [dbo].[Расход]
                                       where [НомерПН]=@ClientOrderId";
            orderChangeDates.AddRange((await _db.QueryAsync<DateTime>(selectOrderItemsChangedDateQuery, queryParams, transaction)).ToList());
            
            await _erpChangedRepository.Save(
                clientOrderMapping.LegacyId,
                orderChangeDates.Max(),
                typeof(ClientOrder).Name
            );
        }

        private async Task UpdateMarkVipolneno(int clientOrderId, IDbTransaction transaction)
        {
            var getConnectedDocumentsQuery = @"select * from [dbo].[connected_documents] 
                                             where [doc1ID]=@ClientOrderId 
                                             and [type1]=1 
                                             and [type2]=16 
                                             or [doc2ID]=@ClientOrderId 
                                             and[type2]=1 
                                             and [type1]=16";
            var connectedDocuments = (await _db.QueryAsync<ConnectedDocumentsEF>(getConnectedDocumentsQuery, new
            {
                ClientOrderId = clientOrderId
            }, transaction)).ToList();

            if (!connectedDocuments.Any())
            {
                return;
            }

            foreach (var document in connectedDocuments)
            {
                var deliveryId = document.Type1 == 1 ? document.Doc2Id : document.Doc1Id;
                var getDeliveryCargoInvoiceQuery = @"select [CargoInvoice] from [dbo].[dostavka]
                                                   where id=@DeliveryId";
                var deliveryCargoInvoice = (await _db.QueryAsync<string>(getDeliveryCargoInvoiceQuery, new
                {
                    DeliveryId = deliveryId
                }, transaction)).FirstOrDefault();

                if (!string.IsNullOrEmpty(deliveryCargoInvoice))
                {
                    var setCargoInvoiceQuery = @"update [dbo].[dostavka]  
                                               set [vipolneno]=1 
                                               where [id]=@Id";
                    await _db.ExecuteAsync(setCargoInvoiceQuery, new
                    {
                        Id = deliveryId
                    }, transaction);
                }
            }
        }

        private async Task<ProductInfo> GetProductInfo(int productId, IDbTransaction transaction)
        {
            var selectProductSqlQuery = @"select [Марка] as Brand, [КодТипа] as ProductTypeId from [dbo].[Товары] 
                                        where [КодТовара] = @ProductId";
            return await _db.QueryFirstAsync<ProductInfo>(selectProductSqlQuery, new
            {
                ProductId = productId
            }, transaction);
        }
    }
}