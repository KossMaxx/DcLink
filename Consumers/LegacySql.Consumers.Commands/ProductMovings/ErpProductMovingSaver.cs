using Dapper;
using LegacySql.Domain.ProductMoving;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Warehouses;
using MassTransit;
using MessageBus.ProductMovings.Export;
using MessageBus.ProductMovings.Export.Change;
using MessageBus.ProductMovings.Import;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.ProductMovings
{
    public class ErpProductMovingSaver
    {
        private readonly IDbConnection _db;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private readonly IProductMovingMapRepository _productMovingMapRepository;
        private readonly IBus _bus;
        private readonly ILegacyProductMovingRepository _productMovingRepository;
        private ErpProductMovingDto _productMoving;
        private ExternalMap _productMovingMapping;
        private ExternalMap _outWarehouseMapping;
        private ExternalMap _inWarehouseMapping;
        private Dictionary<Guid, ExternalMap> _productMappings;
        public ErpProductMovingSaver(
            IDbConnection db,
            IProductMapRepository productMapRepository,
            IWarehouseMapRepository warehouseMapRepository,
            IProductMovingMapRepository productMovingMapRepository,
            IBus bus, 
            ILegacyProductMovingRepository productMovingRepository)
        {
            _db = db;
            _productMapRepository = productMapRepository;
            _warehouseMapRepository = warehouseMapRepository;
            _productMovingMapRepository = productMovingMapRepository;
            _productMappings = new Dictionary<Guid, ExternalMap>();
            _bus = bus;
            _productMovingRepository = productMovingRepository;
        }

        public void InitErpObject(ErpProductMovingDto entity, ExternalMap productMovingMapping)
        {
            _productMoving = entity;
            _productMovingMapping = productMovingMapping;
        }
        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            _outWarehouseMapping = await _warehouseMapRepository.GetByErpAsync(_productMoving.OutWarehouseId);
            if (_outWarehouseMapping == null)
            {
                why.Append($"Маппинг склада id:{_productMoving.OutWarehouseId} не найден\n");
            }
            _inWarehouseMapping = await _warehouseMapRepository.GetByErpAsync(_productMoving.InWarehouseId);
            if (_inWarehouseMapping == null)
            {
                why.Append($"Маппинг склада id:{_productMoving.InWarehouseId} не найден\n");
            }

            foreach(var item in _productMoving.Items)
            {
                var itemMapping = await _productMapRepository.GetByErpAsync(item.ProductId);
                if(itemMapping == null)
                {
                    why.Append($"Маппинг товара id:{item.ProductId} не найден\n");
                }
                else
                {
                    if (!_productMappings.ContainsKey(item.ProductId))
                    {
                        _productMappings.Add(item.ProductId, itemMapping);
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

        public async Task Create(Guid messageGuid)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var firmInfo = await GetCompany(_productMoving.Okpo, transaction);

                var insertQuery = @"insert into [dbo].[PRN]
                                    ([дата],[юзер],[откуда],[куда],[прим],[spdfl],[ф],[O],[S])
                                    values (@Date,@CreatorUsername,@OutWarehouseId,@InWarehouseId,@Description,@CompanyId,@IsAccepted,@IsShipped,@IsForShipment);
                                    select cast(SCOPE_IDENTITY() as int)";
                var newProductMovingId = (await _db.QueryAsync<int>(insertQuery, new
                {
                    Date = _productMoving.Date,
                    CreatorUsername = _productMoving.CreatorUsername,
                    OutWarehouseId = _outWarehouseMapping.LegacyId,
                    InWarehouseId = _inWarehouseMapping.LegacyId,
                    Description = _productMoving.Description,
                    CompanyId = firmInfo?.Id,
                    IsAccepted = false,
                    IsShipped = false,
                    IsForShipment = false
                }, transaction)).FirstOrDefault();

                if (_productMoving.Items.Any())
                {
                    await SetItems(newProductMovingId, transaction);
                }

                var updateSqlQuery = @"update [dbo].[PRN] 
                                       set 
                                        [ф]=@IsAccepted,
                                        [O]=@IsShipped,
                                        [S]=@IsForShipment
                                       where [prn_ID]=@Id";
                await _db.QueryAsync(updateSqlQuery, new
                {
                    Id = newProductMovingId,
                    IsAccepted = false,
                    IsShipped = true,
                    IsForShipment = true
                }, transaction);

                transaction.Commit();

                await _productMovingMapRepository.SaveAsync(new ExternalMap(messageGuid, newProductMovingId, _productMoving.Id));
                await PublishNewProductMoving(newProductMovingId);
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _db.Close();
            }
        }

        public async Task Update()
        {
            //_db.Open();
            //using var transaction = _db.BeginTransaction();
            //try
            //{
            //    var firmInfo = await GetCompany(_productMoving.Okpo, transaction);

            //    var insertQuery = @"update [dbo].[PRN]
            //                        set 
            //                        [дата]=@Date,
            //                        [юзер]=@CreatorUsername,
            //                        [откуда]=@OutWarehouseId,
            //                        [куда]=@InWarehouseId,
            //                        [прим]=@Description,
            //                        [spdfl]=@CompanyId,
            //                        [ф]=@IsAccepted,
            //                        [O]=@IsShipped,
            //                        [S]=@IsForShipment
            //                        where [prn_ID]=@Id";
            //    var newProductMovingId = (await _db.QueryAsync<int>(insertQuery, new
            //    {
            //        Id = _productMovingMapping.LegacyId,
            //        Date = _productMoving.Date,
            //        CreatorUsername = _productMoving.CreatorUsername,
            //        OutWarehouseId = _outWarehouseMapping.LegacyId,
            //        InWarehouseId = _inWarehouseMapping.LegacyId,
            //        Description = _productMoving.Description,
            //        CompanyId = firmInfo?.Id,
            //        IsAccepted = _productMoving.IsAccepted,
            //        IsShipped = _productMoving.IsShipped,
            //        IsForShipment = _productMoving.IsForShipment
            //    }, transaction)).FirstOrDefault();

            //    if(!_productMoving.IsAccepted || !_productMoving.IsShipped || !_productMoving.IsForShipment)
            //    {
            //        await UpdateItems(transaction);
            //    }                
                
            //    transaction.Commit();
            //}
            //catch (Exception e)
            //{
            //    transaction.Rollback();
            //    throw;
            //}
            //finally
            //{
            //    _db.Close();
            //}
        }

        private async Task SetItems(int movingId, IDbTransaction transaction)
        {
            var productIds = _productMappings.Select(e=>e.Value.LegacyId);
            var productsInfo = await GetProductInfo(productIds, transaction);

            foreach(var item in _productMoving.Items)
            {
                var insertQuery = @"insert into [dbo].[move]
                                    ([prn_ID],[КодТовара],[Количество],[КодТипа],[марка])
                                    values (@MovingId,@ProductId,@Amount,@TypeId,@Brand)";

                var productId = _productMappings[item.ProductId].LegacyId;
                await _db.ExecuteAsync(insertQuery, new
                {
                    MovingId = movingId,
                    ProductId = productId,
                    Amount = item.Amount,
                    TypeId = productsInfo[productId].TypeId,
                    Brand = productsInfo[productId].Brand
                }, transaction);
            }            
        }

        //private async Task UpdateItems(IDbTransaction transaction)
        //{
        //    var selectQuery = @"select [КодТовара] from [dbo].[move]
        //                        where [prn_ID]=@ProductMovingId";
        //    var currentProductIds = await _db.QueryAsync<int>(selectQuery, new
        //    {
        //        ProductMovingId = _productMovingMapping.LegacyId
        //    }, transaction);

        //    var innerProductIds = _productMappings.Select(e=>e.Value.LegacyId);
        //    var productIdsForDelete = currentProductIds.Except(innerProductIds);
        //    var productIdsForUpdate = currentProductIds.Intersect(innerProductIds);
        //    var productIdsForCreate = innerProductIds.Except(currentProductIds);

        //    var deleteQuery = $@"delete from [dbo].[move]
        //                        where [prn_ID]=@ProductMovingId and [КодТовара] in ({string.Join(",", productIdsForDelete)})";
        //    await _db.ExecuteAsync(deleteQuery, new
        //    {
        //        ProductMovingId = _productMovingMapping.LegacyId
        //    }, transaction);


        //    var updateQuery = @"update [dbo].[move]
        //                        set [Количество]=@Amount
        //                        where @[prn_ID]=@ProductMovingId and [КодТовара]=@ProductId";
            
        //    foreach(var productId in productIdsForUpdate)
        //    {
        //        var productErpId = _productMappings.First(e=>e.Value.LegacyId == productId).Key;
        //        await _db.ExecuteAsync(deleteQuery, new
        //        {
        //            ProductMovingId = _productMovingMapping.LegacyId,
        //            ProductId = productId,
        //            Amount = productErpId
        //        }, transaction);
        //    }

        //    var productsInfo = await GetProductInfo(productIdsForCreate, transaction);
        //    foreach (var productId in productIdsForCreate)
        //    {
        //        var insertQuery = @"insert into [dbo].[move]
        //                            ([prn_ID],[КодТовара],[Количество],[КодТипа],[марка])
        //                            values (@MovingId,@ProductId,@Amount,@TypeId,@Brand)";
        //        var productErpId = _productMappings.FirstOrDefault(e => e.Value.LegacyId == productId).Key;
        //        await _db.ExecuteAsync(insertQuery, new
        //        {
        //            MovingId = _productMovingMapping.LegacyId,
        //            ProductId = productId,
        //            Amount = _productMoving.Items.First(e=>e.ProductId == productErpId).Amount,
        //            TypeId = productsInfo[productId].TypeId,
        //            Brand = productsInfo[productId].Brand
        //        }, transaction);
        //    }            
        //}

        private async Task<Dictionary<int, ProductInfo>> GetProductInfo(IEnumerable<int> productIds, IDbTransaction transaction)
        {
            var selectQuery = $@"select [КодТовара] as Id, [Марка] as Brand,[КодТипа] as TypeId from [dbo].[Товары]
                                where [КодТовара] in ({string.Join(",", productIds)})";
            var productInfoList = await _db.QueryAsync<ProductInfo>(selectQuery, transaction:transaction);
            return productInfoList.ToDictionary(e => e.Id, e => e);
        }

        private async Task<FirmInfo> GetCompany(string okpo, IDbTransaction transaction)
        {
            var selectFirmQuery = @"select [ID] as Id, [NAME_ukr] as Name from [dbo].[OOO]
                                    where [okpo]=@Okpo";
            var firmInfo = await _db.QueryFirstOrDefaultAsync<FirmInfo>(selectFirmQuery, new
            {
                Okpo = okpo
            }, transaction);

            return firmInfo;
        }

        private async Task PublishNewProductMoving(int id)
        {
            var productMoving = await _productMovingRepository.GetProductMovingAsync(id, CancellationToken.None);
            if (productMoving == null)
            {
                throw new KeyNotFoundException("Трансфер не найден");
            }

            var productMovingDto = new ProductMovingDto
            {
                Code = productMoving.Id.InnerId,
                Date = productMoving.Date,
                CreatorUsername = productMoving.CreatorUsername,
                CreatorId = productMoving.CreatorId?.ExternalId,
                OutWarehouseId = productMoving.OutWarehouseId.ExternalId.Value,
                InWarehouseId = productMoving.InWarehouseId.ExternalId.Value,
                Description = productMoving.Description,
                Okpo = productMoving.Okpo,
                IsAccepted = productMoving.IsAccepted,
                IsForShipment = productMoving.IsForShipment,
                IsShipped = productMoving.IsShipped,
                Items = productMoving.Items.Select(e => new ProductMovingItemDto
                {
                    ProductId = e.ProductId.ExternalId.Value,
                    Amount = e.Amount,
                    Price = e.Price
                })
            };

            await _bus.Publish(new ChangeLegacyProductMovingMessage
            {
                SagaId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                Value = productMovingDto,
                ErpId = _productMoving.Id
            });
        }

        private class ProductInfo
        {
            public int Id { get; set; }
            public int TypeId { get; set; }
            public string Brand { get; set; }
        }
    }
}
