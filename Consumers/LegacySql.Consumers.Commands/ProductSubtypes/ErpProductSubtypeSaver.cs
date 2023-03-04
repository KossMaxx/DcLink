using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using MessageBus.ProductSubtypes.Import;

namespace LegacySql.Consumers.Commands.ProductSubtypes
{
    public class ErpProductSubtypeSaver
    {
        private readonly IDbConnection _db;
        private readonly IProductSubtypeMapRepository _productSubtypeMapRepository;
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private ExternalMap _productTypeMap;
        private ProductSubtypeMap _subtypeMapping;
        private ErpProductSubtypeDto _subtype;

        public ErpProductSubtypeSaver(IDbConnection db, 
            IProductSubtypeMapRepository productSubtypeMapRepository, 
            IProductTypeMapRepository productTypeMapRepository)
        {
            _db = db;
            _productSubtypeMapRepository = productSubtypeMapRepository;
            _productTypeMapRepository = productTypeMapRepository;
        }

        public void InitErpObject(ErpProductSubtypeDto entity, ProductSubtypeMap subtypeMapping)
        {
            _subtype = entity;
            _subtypeMapping = subtypeMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            _productTypeMap = await _productTypeMapRepository.GetByErpAsync(_subtype.ProductTypeId);
            if (_productTypeMap == null)
            {
                why.Append($"Маппинг типа продукта id:{_subtype.ProductTypeId} не найден\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Update()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var updateSubtypeQuery = @"update [dbo].[podtip]
                                         set [nazv]=@Title,[tip]=@TypeId
                                         where [id]=@Id";
                await _db.ExecuteAsync(updateSubtypeQuery, new
                {
                    Id = _subtypeMapping.LegacyId,
                    Title = _subtype.Title,
                    TypeId = _productTypeMap.LegacyId
                }, transaction);

                var updateProductQuery = @"update [dbo].[Товары] 
                                         set [подтип]=@NewTitle
                                         where [подтип]=@OldTitle";
                await _db.ExecuteAsync(updateProductQuery, new
                {
                    NewTitle = _subtype.Title,
                    OldTitle = _subtypeMapping.Title
                }, transaction);

                transaction.Commit();

                await _productSubtypeMapRepository.SaveAsync(
                    new ProductSubtypeMap(
                        _subtypeMapping.MapId,
                        _subtypeMapping.LegacyId,
                        _subtype.Title,
                        _subtypeMapping.ExternalMapId,
                        _subtypeMapping.Id),
                    _subtypeMapping.Id
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

        public async Task Create(Guid messageGuid)
        {
            var insertQuery = @"insert into [dbo].[podtip]
                              ([nazv],[tip]) 
                              values (@Title,@TypeId);
                              select cast(SCOPE_IDENTITY() as int)";
            var newId = (await _db.QueryAsync<int>(insertQuery, new
            {
                Title = _subtype.Title,
                TypeId = _productTypeMap.LegacyId
            })).FirstOrDefault();

            await _productSubtypeMapRepository.SaveAsync(new ProductSubtypeMap(messageGuid, newId, _subtype.Title, _subtype.Id));
        }
    }
}
