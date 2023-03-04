using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Domain.Classes;
using LegacySql.Domain.Employees;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.Products;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.ProductTypeCategories;
using LegacySql.Domain.ProductTypeCategoryParameters;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.ConsumerCommandContracts;
using LegacySql.Legacy.Data.Models;
using MassTransit;
using MessageBus.Products.Export;
using MessageBus.Products.Export.Change;
using MessageBus.Products.Import;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Consumers.Commands.Products
{
    public class ErpProductSaver
    {
        private readonly IDbConnection _db;
        private readonly IEmployeeMapRepository _employeeMapRepository;
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly IProductSubtypeMapRepository _productSubtypeMapRepository;
        private readonly IProductTypeCategoryMapRepository _productTypeCategoryMapRepository;
        private readonly IProductTypeCategoryParameterMapRepository _productTypeCategoryParameterMapRepository;
        private readonly IProductMapRepository _productMapRepository;
        private readonly IClassMapRepository _classMapRepository;
        private ProductTypeMap _productTypeMapping;
        private ProductSubtypeMap _productSubtypeMapping;
        private ExternalMap _productManagerMapping;
        private Dictionary<int, int> _parameterIds = new Dictionary<int, int>();
        private ErpProductDto _product;
        private ExternalMap _productMapping;
        private ClassMap _classMapping;
        private readonly AppDbContext _mapDb;
        private readonly IBus _bus;
        private readonly IProductStore _store;

        public ErpProductSaver(IDbConnection db,
            IEmployeeMapRepository employeeMapRepository,
            IProductTypeMapRepository productTypeMapRepository,
            IProductSubtypeMapRepository productSubtypeMapRepository,
            IProductTypeCategoryMapRepository productTypeCategoryMapRepository,
            IProductTypeCategoryParameterMapRepository productTypeCategoryParameterMapRepository,
            IProductMapRepository productMapRepository,
            IClassMapRepository classMapRepository,
            AppDbContext mapDb,
            IBus bus, 
            IProductStore store)
        {
            _db = db;
            _employeeMapRepository = employeeMapRepository;
            _productTypeMapRepository = productTypeMapRepository;
            _productSubtypeMapRepository = productSubtypeMapRepository;
            _productTypeCategoryMapRepository = productTypeCategoryMapRepository;
            _productTypeCategoryParameterMapRepository = productTypeCategoryParameterMapRepository;
            _productMapRepository = productMapRepository;
            _classMapRepository = classMapRepository;
            _mapDb = mapDb;
            _bus = bus;
            _store = store;
        }

        public void InitErpObject(ErpProductDto product, ExternalMap productMapping)
        {
            _product = product;
            this._productMapping = productMapping;
        }

        public async Task<bool> CheckMapping()
        {
            if (_product.ManagerId.HasValue)
            {
                _productManagerMapping = await _employeeMapRepository.GetByErpAsync(_product.ManagerId.Value);
                if (_productManagerMapping == null)
                {
                    return false;
                }
            }

            if (_product.TypeId.HasValue)
            {
                _productTypeMapping = await _productTypeMapRepository.GetByErpAsync(_product.TypeId.Value);
                if (_productTypeMapping == null)
                {
                    return false;
                }
            }

            if (_product.SubtypeId.HasValue)
            {
                _productSubtypeMapping = await _productSubtypeMapRepository.GetByErpAsync(_product.SubtypeId.Value);
                if (_productSubtypeMapping == null)
                {
                    return false;
                }
            }

            if (_product.Class.HasValue)
            {
                _classMapping = await _classMapRepository.GetByErpAsync(_product.Class.Value);
                if (_classMapping == null)
                {
                    return false;
                }
            }

            if (_product.Parameters != null && _product.Parameters.Any())
            {
                foreach (var parameter in _product.Parameters)
                {
                    var categoryMapping = await _productTypeCategoryMapRepository.GetByErpAsync(parameter.CategoryId);
                    var categoryParameterMapping = await _productTypeCategoryParameterMapRepository.GetByErpAsync(parameter.ParameterId);

                    if (categoryMapping == null)
                    {
                        return false;
                    }

                    if (categoryParameterMapping == null)
                    {
                        return false;
                    }

                    if (!_parameterIds.ContainsKey(categoryMapping.LegacyId))
                    {
                        _parameterIds.Add(categoryMapping.LegacyId, categoryParameterMapping.LegacyId);
                    }
                }
            }


            return true;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            if (_product.ManagerId.HasValue)
            {
                _productManagerMapping = await _employeeMapRepository.GetByErpAsync(_product.ManagerId.Value);
                if (_productManagerMapping == null)
                {
                    why.Append($"Маппинг менеджера продукта id:{_product.ManagerId} не найден\n");
                }
            }

            if (_product.TypeId.HasValue)
            {
                _productTypeMapping = await _productTypeMapRepository.GetByErpAsync(_product.TypeId.Value);
                if (_productTypeMapping == null)
                {
                    why.Append($"Маппинг для типа продуктов id:{_product.TypeId} не найден\n");
                }
            }

            if (_product.SubtypeId.HasValue)
            {
                _productSubtypeMapping = await _productSubtypeMapRepository.GetByErpAsync(_product.SubtypeId.Value);
                if (_productSubtypeMapping == null)
                {
                    why.Append($"Маппинг для подтипа продуктов id:{_product.SubtypeId} не найден\n");
                }
            }

            if (_product.Class.HasValue)
            {
                _classMapping = await _classMapRepository.GetByErpAsync(_product.Class.Value);
                if (_classMapping == null)
                {
                    why.Append($"Маппинг для класса продукта id:{_product.Class} не найден\n");
                }
            }

            if (_product.Parameters != null && _product.Parameters.Any())
            {
                var notCategoryMappings = new List<Guid>();
                var notCategoryParameterMappings = new List<Guid>();

                foreach (var parameter in _product.Parameters)
                {
                    var categoryMapping = await _productTypeCategoryMapRepository.GetByErpAsync(parameter.CategoryId);
                    var categoryParameterMapping = await _productTypeCategoryParameterMapRepository.GetByErpAsync(parameter.ParameterId);

                    if (categoryMapping == null)
                    {
                        notCategoryMappings.Add(parameter.CategoryId);
                    }

                    if (categoryParameterMapping == null)
                    {
                        notCategoryParameterMappings.Add(parameter.CategoryId);
                    }

                    if (categoryMapping != null && categoryParameterMapping != null)
                    {
                        if (!_parameterIds.ContainsKey(categoryMapping.LegacyId))
                        {
                            _parameterIds.Add(categoryMapping.LegacyId, categoryParameterMapping.LegacyId);
                        }
                    }
                }

                if (notCategoryMappings.Any() || notCategoryParameterMappings.Any())
                {
                    why.Append("Маппинг для категорий id:");
                    notCategoryMappings.ForEach(e => { why.Append($"{e},"); });
                    why.Append("не найден\n");

                    why.Append("Маппинг для параметров id:");
                    notCategoryParameterMappings.ForEach(e => { why.Append($"{e},"); });
                    why.Append("не найден\n");
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
            int newProductId = 0;
            
            var manufactureMapping = _product.ManufactureId.HasValue
                ? await _mapDb.ManufacturerMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == _product.ManufactureId.Value)
                : null;
            var manufacture = manufactureMapping != null ? manufactureMapping.LegacyTitle : string.Empty;

            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                if (_productMapping == null)
                {
                    newProductId = await CreateProduct(_product, manufacture, transaction);                    
                }
                else
                {
                    await UpdateProduct(_product, manufacture, transaction);
                }
                if (_product.Parameters != null && _product.Parameters.Any())
                {
                    await UpdateProductParameters(_parameterIds, _productMapping?.LegacyId ?? newProductId, transaction);
                }

                await UpdateProductDescriptions(_productMapping?.LegacyId ?? newProductId, _product, transaction);

                await UpdateProductPictures(_productMapping?.LegacyId ?? newProductId, _product.PicturesUrls, transaction);

                transaction.Commit();

                await _productMapRepository.SaveAsync(new ExternalMap(messageId, newProductId, _product.Id));
                await PublishNewProduct(newProductId);
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


        private async Task<int> CreateProduct(ErpProductDto product, string manufacture, IDbTransaction transaction)
        {
            var managerNickname = await GetManagerNickname(transaction);
            var productCountryId = await GetCountryIdByIso(product.ProductCountryIsoCode, transaction);
            var brandCountryId = await GetCountryIdByIso(product.BrandCountryIsoCode, transaction);

            var newProductId = await _store.Create(product.Title,
                product.Title,
                _productTypeMapping.LegacyId,
                _productSubtypeMapping?.Title,
                _classMapping != null ? _classMapping.LegacyTitle : "",
                manufacture,
                null,
                product.VendorCode,
                product.NomenclatureBarcode,
                product.NameForPrinting,
                productCountryId,
                brandCountryId, 
                product.Weight,
                product.Volume,
                product.Guarantee,
                product.GuaranteeIn,
                product.Unit,
                product.Vat,
                product.IsImported,
                product.NomenclatureCode,
                product.IsProductIssued,
                product.IsDistribution,
                product.ScanMonitoring,
                product.ScanHotline,
                product.Game,
                product.ManualRrp,
                product.NotInvolvedInPricing,
                product.Monitoring,
                product.Price,
                product.Markdown,
                product.ManufactureSiteLink,
                product.CurrencyId,
                managerNickname,
                _productManagerMapping?.LegacyId,
                product.Height,
                product.Width,
                product.Depth,
                transaction);

           return newProductId;
        }

        private async Task UpdateProductParameters(Dictionary<int, int> parameterIds, int productId,IDbTransaction transaction)
        {
            var getQuery = @"select * from [dbo].[KategoryTovars]
                                     where [kodtovara]=@ProductId";
            var productCategories = (await _db.QueryAsync(getQuery, new {ProductId = productId}, transaction))
                .Select(e => new ProductCategoryParameterEF
                {
                    Id = e.id,
                    CategoryId = e.katID,
                    ParameterId = e.param,
                    ProductId = e.kodtovara
                });
            foreach (var parameter in parameterIds)
            {
                var productCategory = productCategories.FirstOrDefault(e => e.CategoryId == parameter.Key);
                if (productCategory != null)
                {
                    await _store.UpdateParameter(productCategory.Id, productId, parameter.Key, parameter.Value, transaction);
                }
                else
                {
                    await _store.InsertParameter(productId, parameter.Key, parameter.Value, transaction);
                }
            }
        }

        private async Task UpdateProduct(ErpProductDto product, string manufacture, IDbTransaction transaction)
        {
            var managerNickname = await GetManagerNickname(transaction);
            var productCountryId = await GetCountryIdByIso(product.ProductCountryIsoCode, transaction);
            var brandCountryId = await GetCountryIdByIso(product.BrandCountryIsoCode, transaction);

            await _store.Update(
                _productMapping.LegacyId,
                product.Title,
                product.Title,
                _productTypeMapping.LegacyId,
                _productSubtypeMapping?.Title,
                _classMapping != null ? _classMapping.LegacyTitle : "",
                manufacture,
                null,
                product.VendorCode,
                product.NomenclatureBarcode,
                product.NameForPrinting,
                productCountryId,
                brandCountryId,
                product.Weight,
                product.Volume,
                product.Guarantee,
                product.GuaranteeIn,
                product.Unit,
                product.Vat,
                product.IsImported,
                product.NomenclatureCode,
                product.IsProductIssued,
                product.IsDistribution,
                product.ScanMonitoring,
                product.ScanHotline,
                product.Game,
                product.ManualRrp,
                product.NotInvolvedInPricing,
                product.Monitoring,
                product.Price,
                product.Markdown,
                product.ManufactureSiteLink,
                product.CurrencyId,
                managerNickname,
                _productManagerMapping?.LegacyId,
                product.Height,
                product.Width,
                product.Depth,
                transaction);
        }

        private async Task UpdateProductDescriptions(int productId, ErpProductDto product,IDbTransaction transaction)
        {
            await _store.UpdateDescriptions(productId, 1, product.DescriptionRu, transaction);
            await _store.UpdateDescriptions(productId, 2, product.DescriptionUa, transaction);
        }

        private async Task UpdateProductPictures(int productId, IEnumerable<string> picturesUrls, IDbTransaction transaction)
        {
            if (picturesUrls == null)
            {
                return;
            }

            var sql = @"delete from dbo.[PicturesUrl]
                        where tovID = @Id";

            await _db.ExecuteAsync(sql, new { Id = productId }, transaction);

            var insertProductQuery = @"insert into dbo.[PicturesUrl]
                                         ([tovID],[url],[ddd])
                                         values (@ProductId,@Url,@Date)";
            foreach (var url in picturesUrls)
            {
                await _db.QueryAsync(insertProductQuery,
                    new
                    {
                        ProductId = productId,
                        Url = url,
                        Date = DateTime.Now,
                    },
                    transaction
                );
            }
            transaction.Commit();
        }

        private async Task<string> GetManagerNickname(IDbTransaction transaction)
        {
            var getManagerQuery = @"select [uuu] from [dbo].[Сотрудники]
                                  where [КодСотрудника]=@ProductManagerId";

            var managerNickname = await _db.QueryFirstOrDefaultAsync<string>(getManagerQuery, new
            {
                ProductManagerId = _productManagerMapping?.LegacyId
            }, transaction);

            return managerNickname;
        }

        private async Task PublishNewProduct(int newProductId)
        {
            var productDto = MapToDto(newProductId);
            await _bus.Publish(new ChangeLegacyProductMessage
            {
                MessageId = Guid.NewGuid(), 
                Value = productDto, 
                ErpId = _product.Id
            });
        }

        private ProductDto MapToDto(int newProductId)
        {
            return new ProductDto
            {
                Code = newProductId,
                Brand = _product.Title,
                WorkName = _product.Title,
                SubtypeId = _product.SubtypeId,
                ManufactureId = _product.ManufactureId,
                VendorCode = _product.VendorCode,
                Weight = _product.Weight,
                Volume = _product.Volume,
                Guarantee = _product.Guarantee,
                GuaranteeIn = _product.GuaranteeIn,
                ManualRrp = _product.ManualRrp,
                NotInvolvedInPricing = _product.NotInvolvedInPricing,
                Monitoring = _product.Monitoring,
                Price = _product.Price,
                ProductTypeId = _product.TypeId,
                Parameters = _product.Parameters.Select(p => new ProductCategoryParameterDto
                {
                    CategoryId = p.CategoryId,
                    ParameterId = p.ParameterId
                }),
                ManagerId = _product.ManagerId,
                ManufactureSiteLink = _product.ManufactureSiteLink,
                DescriptionRu = _product.DescriptionRu,
                DescriptionUa = _product.DescriptionUa,
                Height = _product.Height,
                Width = _product.Width,
                Depth = _product.Depth,
                Pictures = _product.PicturesUrls
            };
        }

        private async Task<int> GetCountryIdByIso(string iso, IDbTransaction transaction)
        {
            if (string.IsNullOrEmpty(iso))
            {
                throw new ApplicationException("Поле с Iso страны должно быть заполнено");
            }
            var selectSqlQuery = @"select [C_ID] from [dbo].[TBL_Countries]
                                 where [C_ISO2_code]=@Iso";
            var countryId = await _db.QueryFirstOrDefaultAsync<int?>(selectSqlQuery, new {Iso = iso}, transaction);

            if (!countryId.HasValue)
            {
                throw new ApplicationException($"Страна с Iso {iso} не найдена");
            }

            return countryId.Value;
        } 
    }
}