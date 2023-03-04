using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Products;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Purchases;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Warehouses;
using MessageBus.Purchases.Import;

namespace LegacySql.Consumers.Commands.Purchases
{
    public class ErpPurchaseSaver
    {
        private readonly IDbConnection _db;
        private readonly IPurchaseMapRepository _purchaseMapRepository;
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private ExternalMap _clientMapping;
        private Dictionary<Guid, int> _productTypeMappings;
        private Dictionary<Guid, int> _productMappings;
        private ExternalMap _warehouseMapping;
        private ExternalMap _purchaseMapping;
        private ErpPurchaseDto _purchase;
        private IDbTransaction _transaction;

        public ErpPurchaseSaver(
            IDbConnection db, 
            IProductTypeMapRepository productTypeMapRepository, 
            IProductMapRepository productMapRepository, 
            IPurchaseMapRepository purchaseMapRepository, 
            IClientMapRepository clientMapRepository, 
            IWarehouseMapRepository warehouseMapRepository)
        {
            _db = db;
            _productTypeMapRepository = productTypeMapRepository;
            _productMapRepository = productMapRepository;
            _purchaseMapRepository = purchaseMapRepository;
            _clientMapRepository = clientMapRepository;
            _warehouseMapRepository = warehouseMapRepository;
            _productTypeMappings = new Dictionary<Guid, int>();
            _productMappings = new Dictionary<Guid, int>();
        }

        public void InitErpObject(ErpPurchaseDto _purchase, ExternalMap purchaseMapping)
        {
            this._purchase = _purchase;
            _purchaseMapping = purchaseMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            
            if (_purchase.WarehouseId.HasValue)
            {
                _warehouseMapping = await _warehouseMapRepository.GetByErpAsync(_purchase.WarehouseId.Value);
                if (_warehouseMapping == null)
                {
                    why.Append($"Не найден маппинг для WarehouseId:{_purchase.WarehouseId}\n");
                }
            }

            _clientMapping = await _clientMapRepository.GetByErpAsync(_purchase.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Не найден маппинг для ClientId:{_purchase.ClientId}\n");
            }

            if (_purchase.Items.Any())
            {
                foreach (var item in _purchase.Items)
                {
                    var productMapping = await _productMapRepository.GetByErpAsync(item.ProductId);
                    if (productMapping == null)
                    {
                        why.Append($"Не найден маппинг для ProductId:{item.ProductId}\n");
                    }
                    else
                    {
                        if (!_productMappings.ContainsKey(item.ProductId))
                        {
                            _productMappings.Add(item.ProductId, productMapping.LegacyId);
                        }
                    }

                    if (item.ProductTypeId.HasValue)
                    {
                        var productTypeMapping = await _productTypeMapRepository.GetByErpAsync(item.ProductTypeId.Value);
                        if (productTypeMapping == null)
                        {
                            why.Append($"Не найден маппинг для ProductTypeId:{item.ProductTypeId}\n");
                        }
                        else
                        {
                            if (!_productTypeMappings.ContainsKey(item.ProductTypeId.Value))
                            {
                                _productTypeMappings.Add(item.ProductTypeId.Value, productTypeMapping.LegacyId);
                            }
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

        public async Task SaveErpObject(Guid messageId)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            _transaction = transaction;
            try
            {
                if (_purchaseMapping == null)
                {
                    var newPurchaseId = await CreatePurchase();
                    await CreatePurchaseItems(newPurchaseId);
                    transaction.Commit();
                    await _purchaseMapRepository.SaveAsync(new ExternalMap(messageId, newPurchaseId, _purchase.Id));
                }
                else
                {
                    await UpdatePurchase();
                    await UpdatePurchaseItem();
                    transaction.Commit();
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

        private async Task<int> CreatePurchase()
        {
            var query = @"insert into dbo.[ПН]
                        ([Дата],[Сумма],[Описание],[Отдел],[тип],[ф],[klientID],[Кол])
                        values (@Date,@Amount,@Description,@WarehouseId,0,1,@ClientId,@Quantity);
                        select cast(SCOPE_IDENTITY() as int)";

            var queryParams = new
            {
                Date= _purchase.Date,
                Amount = _purchase.Amount,
                Description=_purchase.Description,
                WarehouseId = _warehouseMapping.LegacyId,
                ClientId=_clientMapping.LegacyId,
                Quantity= _purchase.Quantity
            };

            return (await _db.QueryAsync<int>(query, queryParams, _transaction)).FirstOrDefault();
        }

        private async Task CreatePurchaseItems(int purchaseId)
        {
            var query = @"insert into [dbo].[Приход]
                        ([НомерПН], [КодТовара], [КодТипа],[марка],[Цена],[Количество])
                        values (@PurchaseId,@ProductId,@ProductTypeId,@Brand,@Price,@Quantity);
                        select cast(SCOPE_IDENTITY() as int)";
            
            foreach (var item in _purchase.Items)
            {
                var queryParams = new
                {
                    PurchaseId=purchaseId,
                    ProductId=_productMappings[item.ProductId],
                    ProductTypeId= item.ProductTypeId.HasValue ? (int?)_productTypeMappings[item.ProductTypeId.Value] : null,
                    Brand=item.ProductTitle,
                    Price=item.Price,
                    Quantity=item.Quantity
                };
                var operationId = (await _db.QueryAsync<int>(query, queryParams, _transaction)).FirstOrDefault();

                await CreateSerialNumbers(purchaseId, operationId, item.SerialNumbers);
            }
        }

        private async Task CreateSerialNumbers(int purchaseId, int operationId, IEnumerable<string> serialNumbers)
        {
            var query = @"insert into [dbo].[sernom]
                        ([N_PN], [выдан], [posID], [серном])
                        values (@PurchaseId,1,@OperationId,@SerialNumber)";
            foreach (var number in serialNumbers)
            {
                var queryParams = new
                {
                    PurchaseId = purchaseId,
                    OperationId = operationId,
                    SerialNumber = number
                };

                await _db.ExecuteAsync(query, queryParams, _transaction);
            }
        }

        private async Task UpdatePurchase()
        {
            var query = @"update dbo.[ПН]
                        set [Дата]=@Date,[Сумма]=@Amount,[Описание]=@Description,[Отдел]=@WarehouseId,[тип]=0,[ф]=1,[klientID]=@ClientId,[Кол]=@Quantity
                        where [НомерПН]=@PurchaseId";

            var queryParams = new
            {
                PurchaseId = _purchaseMapping.LegacyId,
                Date = _purchase.Date,
                Amount = _purchase.Amount,
                Description = _purchase.Description,
                WarehouseId = _warehouseMapping.LegacyId,
                ClientId = _clientMapping.LegacyId,
                Quantity = _purchase.Quantity
            };

            await _db.ExecuteAsync(query, queryParams, _transaction);
        }

        private async Task UpdatePurchaseItem()
        {
            var getCurrentItemsSqlQuery = @"select [КодОперации] as OperationId,[КодТовара] as ProductId from [dbo].[Приход]
                                          where [НомерПН]=@PurchaseId";
            var currentItems = await _db.QueryAsync<CurrentItems>(getCurrentItemsSqlQuery, new
            {
                PurchaseId = _purchaseMapping.LegacyId
            }, _transaction);

            var updateItemSqlQuery = @"update [dbo].[Приход]
                                    set [марка]=@Brand,
                                        [Цена]=@Price,
                                        [Количество]=@Quantity
                                    where [КодОперации]=@OperationId";

            foreach (var item in _purchase.Items)
            {
                var currentItem = currentItems.FirstOrDefault(e => e.ProductId == _productMappings[item.ProductId]);
                if (currentItem != null)
                {
                    var queryParams = new
                    {
                        OperationId = currentItem.OperationId,
                        Brand = item.ProductTitle,
                        Price = item.Price,
                        Quantity = item.Quantity
                    };
                    await _db.ExecuteAsync(updateItemSqlQuery, queryParams, _transaction);
                    if (item.SerialNumbers.Any())
                    {
                        await UpdateSerialNumbers(currentItem.OperationId, item.SerialNumbers);
                    }
                }
                else
                {
                    var insertItemSqlQuery = @"insert into [dbo].[Приход]
                                            ([НомерПН], [КодТовара], [КодТипа],[марка],[Цена],[Количество])
                                            values (@PurchaseId,@ProductId,@ProductTypeId,@Brand,@Price,@Quantity);
                                            select cast(SCOPE_IDENTITY() as int)";

                    var queryParams = new
                    {
                        PurchaseId = _purchaseMapping.LegacyId,
                        ProductId = _productMappings[item.ProductId],
                        ProductTypeId = item.ProductTypeId.HasValue ? (int?)_productTypeMappings[item.ProductTypeId.Value] : null,
                        Brand = item.ProductTitle,
                        Price = item.Price,
                        Quantity = item.Quantity
                    };
                    var operationId = (await _db.QueryAsync<int>(insertItemSqlQuery, queryParams, _transaction)).FirstOrDefault();

                    await CreateSerialNumbers(_purchaseMapping.LegacyId, operationId, item.SerialNumbers);
                }
            }
        }

        private async Task UpdateSerialNumbers(int operationId, IEnumerable<string> serialNumbers)
        {
            var selectCurrentNumbersSqlQuery = @"select [серном] as SerialNumber from [dbo].[sernom]
                                               where [N_PN]=@PurchaseId and [posID]=@OperationId and [серном] is not null ";
            var currentSerialNumbers = (await _db.QueryAsync<string>(selectCurrentNumbersSqlQuery, new
            {
                PurchaseId = _purchaseMapping.LegacyId,
                OperationId = operationId
            }, _transaction)).ToList();

            var numbersForCreate = serialNumbers.Except(currentSerialNumbers);
            await CreateSerialNumbers(_purchaseMapping.LegacyId, operationId, numbersForCreate);
        }
        
       private class CurrentItems
        {
            public int OperationId { get; set; }
            public int ProductId { get; set; }
        }
    }
}