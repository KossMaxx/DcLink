using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Warehouses;
using MessageBus.WarehouseStocks.Import;

namespace LegacySql.Consumers.Commands.WarehouseStocks
{
    public class ErpWarehouseStockSaver
    {
        private readonly IDbConnection _db;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private ErpWarehouseStockDto _erpWarehouseStock;
        private ExternalMap _productMapping;
        private ExternalMap _warehouseMapping;

        public ErpWarehouseStockSaver(IDbConnection db, 
            IProductMapRepository productMapRepository, 
            IWarehouseMapRepository warehouseMapRepository)
        {
            _db = db;
            _productMapRepository = productMapRepository;
            _warehouseMapRepository = warehouseMapRepository;
        }

        public void InitErpObject(ErpWarehouseStockDto erpWarehouseStock)
        {
            _erpWarehouseStock = erpWarehouseStock;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            _productMapping = await _productMapRepository.GetByErpAsync(_erpWarehouseStock.ProductId);
            if (_productMapping == null)
            {
                why.Append($"Маппинг продукта id:{_erpWarehouseStock.ProductId} не найден\n");
            }

            _warehouseMapping = await _warehouseMapRepository.GetByErpAsync(_erpWarehouseStock.WarehouseId);
            if (_warehouseMapping == null)
            {
                why.Append($"Маппинг склада id:{_erpWarehouseStock.WarehouseId} не найден\n");
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
            var getStockIdQuery = @"select [id] from [dbo].[SkladFree]
                                   where [tovID]=@ProductId and [skladID]=@WarehouseId";
            var legacyStockId = (await _db.QueryAsync<int>(getStockIdQuery, new
            {
                ProductId = _productMapping.LegacyId,
                WarehouseId = _warehouseMapping.LegacyId
            })).FirstOrDefault();

            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                if (legacyStockId == 0)
                {
                    await Create(transaction);
                }
                else
                {
                    await Update(legacyStockId, transaction);
                }

                await UpdateStocksOfProduct(transaction);
                await UpdateStocksOfWarehouse(transaction);

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

        private async Task Update(int legacyStockId, IDbTransaction transaction)
        {
            var insertQuery = @"update [dbo].[SkladFree]
                              set [Qfree]=@Quantity
                              where id=@Id";
            await _db.ExecuteAsync(insertQuery, new
            {
                Id = legacyStockId,
                Quantity = _erpWarehouseStock.Quantity
            }, transaction);
        }

        private async Task Create(IDbTransaction transaction)
        {
            var insertQuery = @"insert into [dbo].[SkladFree]
                              ([tovID],[skladID],[Qfree])
                              values (@ProductId,@WarehouseId,@Quantity)";
            await _db.ExecuteAsync(insertQuery, new
            {
                ProductId = _productMapping.LegacyId,
                WarehouseId = _warehouseMapping.LegacyId,
                Quantity = _erpWarehouseStock.Quantity
            }, transaction);
        }

        private async Task UpdateStocksOfProduct(IDbTransaction transaction)
        {
            var selectStocksSqlQuery = @"select [Qfree] from [dbo].[SkladFree]
                                       where [tovID]=@ProductId and [skladID]!=1000 and [skladID]!=1001";
            var stocks = await _db.QueryFirstOrDefaultAsync<int>(selectStocksSqlQuery, new
            {
                ProductId = _productMapping.LegacyId
            }, transaction);

            var updateProductSqlQuery = @"update [dbo].[Товары] set [нал_ф]=@Stocks
                                        where [КодТовара] = @ProductId";
            await _db.ExecuteAsync(updateProductSqlQuery, new
            {
                Stocks = stocks,
                ProductId = _productMapping.LegacyId
            }, transaction);
        }

        private async Task UpdateStocksOfWarehouse(IDbTransaction transaction)
        {
            var selectStockByWarehouseSqlQuery = @"select [id] from [dbo].[Sklad]
                                                where [kodt] = @ProductId and [otdel] = @WarehouseId";
            var stockByWarehouseId = await _db.QueryFirstOrDefaultAsync<int?>(selectStockByWarehouseSqlQuery, new
            {
                ProductId = _productMapping.LegacyId,
                WarehouseId = _warehouseMapping.LegacyId
            }, transaction);

            if (!stockByWarehouseId.HasValue || stockByWarehouseId == 0)
            {
                var insertStockByWarehouseSqlQuery = @"insert into [dbo].[Sklad]
                                                     ([kodt],[otdel],[nal])
                                                     values (@ProductId,@WarehouseId,@Quantity)";
                await _db.ExecuteAsync(insertStockByWarehouseSqlQuery, new
                {
                    ProductId = _productMapping.LegacyId,
                    WarehouseId = _warehouseMapping.LegacyId,
                    Quantity = _erpWarehouseStock.Quantity
                }, transaction);
            }
            else
            {
                var updateStockByWarehouseSqlQuery = @"update [dbo].[Sklad] 
                                                     set [nal]=@Quantity
                                                     where [id]=@Id";
                await _db.ExecuteAsync(updateStockByWarehouseSqlQuery, new
                {
                    Id = stockByWarehouseId,
                    Quantity = _erpWarehouseStock.Quantity
                }, transaction);
            }
        }
    }
}
