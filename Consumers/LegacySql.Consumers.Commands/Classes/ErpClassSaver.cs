using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Classes;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using MessageBus.Classes.Import;

namespace LegacySql.Consumers.Commands.Classes
{
    public class ErpClassSaver
    {
        private readonly IDbConnection _db;
        private readonly IClassMapRepository _classMapRepository;
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly ILegacyClassRepository _legacyClassRepository;
        private ClassMap _classMap;
        private ErpClassDto _productClass;

        public ErpClassSaver(IDbConnection db,
            IClassMapRepository classMapRepository,
            IProductTypeMapRepository productTypeMapRepository,
            ILegacyClassRepository legacyClassRepository)
        {
            _db = db;
            _classMapRepository = classMapRepository;
            _productTypeMapRepository = productTypeMapRepository;
            _legacyClassRepository = legacyClassRepository;
        }

        public void InitErpObject(ErpClassDto entity, ClassMap classMapping)
        {
            _productClass = entity;
            _classMap = classMapping;
        }
        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            foreach (var type in _productClass.ProductTypes)
            {
                var map = await _productTypeMapRepository.GetByErpAsync(type);

                if (map == null)
                {
                    why.Append($"Маппинг типа продукта id:{type} не найден\n");
                }
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Update(CancellationToken cancellationToken)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {

                var updateProductQuery = @"update [dbo].[Товары] 
                                         set [klas]=@NewTitle
                                         where [klas]=@OldTitle";
                var updatePriceAlgoritmDetailsQuery = @"update [dbo].[PriceAlgoritmDetails] 
                                                      set [klas]=@NewTitle
                                                      where [klas]=@OldTitle";
                var queryObject = new
                {
                    NewTitle = _productClass.Title,
                    OldTitle = _classMap.LegacyTitle
                };

                var legacyClass = await _legacyClassRepository.GetAsync(_productClass.Title, cancellationToken);

                var existLegacyMapping = new List<int>();

                foreach (var productType in legacyClass.ProductTypes)
                {
                    if (productType.ExternalId == null) continue;
                    var productTypeMap = await _productTypeMapRepository.GetByErpAsync(productType.ExternalId.Value);
                    existLegacyMapping.Add(productTypeMap.LegacyId);
                }

                if (_productClass.ProductTypes != null && _productClass.ProductTypes.Any())
                {
                    var productTypesMap = new List<ExternalMap>();

                    foreach (var type in _productClass.ProductTypes)
                    {
                        var map = await _productTypeMapRepository.GetByErpAsync(type);
                        productTypesMap.Add(map);
                    }

                    var newLegacyMapping = productTypesMap.Select(map => map.LegacyId).ToList();
                    var oldTypes = existLegacyMapping.Except(newLegacyMapping);
                    var newTypes = newLegacyMapping.Except(existLegacyMapping);

                    if (oldTypes.Any())
                    {
                        foreach (var productType in oldTypes)
                        {
                            await DeleteProductTypeMap(productType, transaction);
                        }
                    }

                    if (newTypes.Any())
                    {
                        foreach (var productType in newTypes)
                        {
                            await InsertProductTypeMap(productType, transaction);
                        }
                    }
                }
                else
                {
                    await DeleteProductTypeMapAll(transaction);
                }

                await _db.ExecuteAsync(updateProductQuery, queryObject, transaction);
                await _db.ExecuteAsync(updatePriceAlgoritmDetailsQuery, queryObject, transaction);

                transaction.Commit();

                await _classMapRepository.SaveAsync(new ClassMap(_classMap.MapId, _productClass.Title, _classMap.ExternalMapId, _classMap.Id), _classMap.Id);
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

        public async Task Create(Guid messageGuid)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                if (_productClass.ProductTypes != null && _productClass.ProductTypes.Any())
                {

                    foreach (var productType in _productClass.ProductTypes)
                    {
                        var _productTypeMap = await _productTypeMapRepository.GetByErpAsync(productType);
                        await InsertProductTypeMap(_productTypeMap.LegacyId, transaction);
                    }
                }

                await _classMapRepository.SaveAsync(
                    new ClassMap(
                        messageGuid,
                        _productClass.Title,
                        _productClass.Id)
                );
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

        private async Task InsertProductTypeMap(int productType, IDbTransaction transaction = null)
        {
            var insertMapQuery = @"INSERT into [dbo].[klas] ([nazv],[tip]) values (@Title, @TypeId); select cast(SCOPE_IDENTITY() as int)"; 
            await _db.ExecuteAsync(insertMapQuery, new { Title = _productClass.Title, TypeId = productType}, transaction);
        }

            private async Task DeleteProductTypeMapAll(IDbTransaction transaction)
        {
            var deleteQuery = @"delete from [dbo].[klas] where nazv = @Title";
            await _db.ExecuteAsync(deleteQuery, new { Title = _productClass.Title}, transaction);
        }

        private async Task DeleteProductTypeMap(int productType, IDbTransaction transaction)
        {
            var deleteQuery = @"delete from [dbo].[klas] where nazv = @Title AND tip = @TypeId";
            await _db.ExecuteAsync(deleteQuery, new { Title = _productClass.Title, TypeId = productType }, transaction);
        }
    }
}
