using System;
using LegacySql.Domain.Products;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using LinqKit;
using MassTransit.Initializers;
using Dapper;
using System.Data;

namespace LegacySql.Legacy.Data.Products
{
    public class ProductRepository : ILegacyProductRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly List<int> _notUsedTypesIds;
        private readonly Expression<Func<ProductEF, bool>> _commonFilter;
        private readonly LegacyDbConnection _sqlConnection;

        public ProductRepository(LegacyDbContext db,
            AppDbContext mapDb,
            ILegacyProductTypeRepository legacyProductTypeRepository,
            LegacyDbConnection sqlConnection)
        {
            _db = db;
            _mapDb = mapDb;
            _notUsedTypesIds = legacyProductTypeRepository.GetNotUsedTypesIds();
            _commonFilter = BuildCommonFilter();
            _sqlConnection = sqlConnection;
        }

        private Expression<Func<ProductEF, bool>> BuildCommonFilter()
        {
            return PredicateBuilder.New<ProductEF>(p => !_notUsedTypesIds.Contains((int)p.ProductTypeId) || p.ProductTypeId == null);
        }

        private async Task<IEnumerable<ProductData>> GetProductsFromDb(DateTime? changedAt = null, IEnumerable<int> idsFilter = null)
        {
            var procedure = "dbo.E21_pkg_get_products_changed_by_checkdate";

            var procedureParams = new
            {
                check_date = changedAt,
                product_code_list = idsFilter == null || !idsFilter.Any() ? null : string.Join(",", idsFilter)
            };

            var productData = await _sqlConnection.Connection.QueryAsync<ProductData, PictureData, VideoData, ProductData>(
                sql: procedure,
                param: procedureParams,
                map: (product, picture, video) =>
                {
                    product.Picture = picture?.Product_pic_Id == 0 ? null : picture;
                    product.Video = video?.Product_video_Id == 0 ? null : video;
                    return product;
                },
                splitOn: "Product_pic_Id, Product_video_Id",
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            return productData;
        }

        private async Task<Dictionary<int, string>> GetCountryIsoFromDb()
        {
            var procedure = "dbo.E21_model_get_countries_iso_code";
            var countryIsoData = await _sqlConnection.Connection.QueryAsync<CountryIsoData>(
                sql: procedure,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            return countryIsoData.ToDictionary(e => e.id, e => e.Code);
        }

        private async Task<IEnumerable<IGrouping<int, ParameterData>>> GetProductCategoryParametersFromDb(IEnumerable<int> idsFilter = null)
        {
            var procedure = "dbo.E21_model_get_products_category_parameter";
            var procedureParams = new
            {
                product_code_list = idsFilter == null || !idsFilter.Any() ? null : string.Join(",", idsFilter)
            };
            var productCategogyParametersData = await _sqlConnection.Connection.QueryAsync<ParameterData>(
                sql: procedure,
                procedureParams,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            return productCategogyParametersData.GroupBy(e => e.ProductId);
        }

        private async Task<Dictionary<int, Dictionary<int, string>>> GetProductDescriptionsFromDb(DateTime? changedAt = null, IEnumerable<int> idsFilter = null)
        {
            var procedure = "dbo.E21_model_get_products_description_changed_by_checkdate";

            var procedureParams = new
            {
                check_date = changedAt,
                product_code_list = idsFilter == null || !idsFilter.Any() ? null : string.Join(",", idsFilter)
            };

            var countryIsoData = await _sqlConnection.Connection.QueryAsync<DescriptionData>(
                sql: procedure,
                param: procedureParams,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            return countryIsoData.GroupBy(e => e.ProductDescription_ProductId)
                .ToDictionary(e => e.Key, e =>
                {
                    var dictionary = new Dictionary<int, string>();
                    foreach (var description in e)
                    {
                        dictionary.TryAdd(description.ProductDescription_LanguageId, description.ProductDescription_Description);
                    }
                    return dictionary;
                });
        }

        public async IAsyncEnumerable<Product> GetChangedProductsAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var productData = await GetProductsFromDb(lastChangedDate, notFullMappingIds);
            if (!productData.Any())
            {
                yield return null;
            }

            var countriesIsoData = await GetCountryIsoFromDb();

            var productCategogyParametersData = await GetProductCategoryParametersFromDb();
            var productsDescriptions = await GetProductDescriptionsFromDb(lastChangedDate);

            var maps = await Maps.Create(productData, countriesIsoData, productCategogyParametersData, _mapDb, cancellationToken);
            var productMapper = new ProductMapper(
                maps.ProductMap,
                maps.ProductTypeMap,
                maps.ManufacturerMap,
                maps.ClassMap,
                maps.SubtypeMap,
                maps.CountryIsoMap,
                maps.ParametersMap,
                maps.ManagerMap,
                productsDescriptions
                );

            foreach (var product in productData.GroupBy(e => e.Product_Code)
                                                .Select(productGroup =>
                                                {
                                                    var masterProduct = productGroup.First();
                                                    var product = productMapper.Map(masterProduct, productGroup.ToList());

                                                    return product;
                                                }))
            {
                yield return product;
            }
        }

        public async Task<Product> GetProductAsync(int id, CancellationToken cancellationToken)
        {
            var proceduresProductIdsParam = new List<int> { id };
            var productData = await GetProductsFromDb(null, proceduresProductIdsParam);
            var countriesIsoData = await GetCountryIsoFromDb();
            var productCategogyParametersData = await GetProductCategoryParametersFromDb(proceduresProductIdsParam);
            var productsDescriptions = await GetProductDescriptionsFromDb(null, proceduresProductIdsParam);

            var maps = await Maps.Create(productData, countriesIsoData, productCategogyParametersData, _mapDb, cancellationToken);
            var productMapper = new ProductMapper(
                maps.ProductMap,
                maps.ProductTypeMap,
                maps.ManufacturerMap,
                maps.ClassMap,
                maps.SubtypeMap,
                maps.CountryIsoMap,
                maps.ParametersMap,
                maps.ManagerMap,
                productsDescriptions
                );

            var masterProduct = productData.FirstOrDefault(e => e.Product_Code == id);
            if (masterProduct == null)
            {
                return null;
            }

            var product = productMapper.Map(masterProduct, productData.Where(c => c.Product_Code == id).ToList());

            return product;
        }

        public async Task<IEnumerable<int>> GetProductsIdsAsync(CancellationToken cancellationToken)
        {
            var filter = PredicateBuilder.New(_commonFilter);
            filter = filter.And(p => !string.IsNullOrEmpty(p.WorkName));

            var count = await _db.Products
                .Where(filter)
                .CountAsync(cancellationToken);
            var result = new List<int>();
            var portion = 100000;
            var cycleLimitation = Math.Ceiling((double)count / portion);
            for (var i = 0; i < cycleLimitation; i++)
            {
                var ids = await _db.Products
                    .Where(filter)
                    .Select(p => p.Code)
                    .Skip(i * portion).Take(portion)
                    .ToListAsync(cancellationToken: cancellationToken);

                result.AddRange(ids);
            }

            return result;
        }

        private class Maps
        {
            public IDictionary<int, Guid?> ProductMap { get; set; }
            public IDictionary<int, Guid?> ProductTypeMap { get; set; }
            public IDictionary<int, Guid?> ManufacturerMap { get; set; }
            public IDictionary<string, Guid?> ClassMap { get; set; }
            public IDictionary<string, SubtypeData> SubtypeMap { get; set; }
            public IDictionary<int, string> CountryIsoMap { get; set; }
            public IDictionary<int, IEnumerable<ProductCategoryParameter>> ParametersMap { get; set; }
            public IDictionary<int, Guid?> ManagerMap { get; set; }

            public static async Task<Maps> Create(
                IEnumerable<ProductData> data,
                IDictionary<int, string> countryIsoMap,
                IEnumerable<IGrouping<int, ParameterData>> productParametersGroups,
                AppDbContext mapDb,
                CancellationToken cancellationToken)
            {
                var uniqProductIds = new List<int>();
                var uniqProductTypeIds = new List<int>();
                var uniqManufacturerIds = new List<int>();
                var uniqClassTitles = new List<string>();
                var uniqSubtypeTitles = new List<string>();
                var uniqManagerIds = new List<int>();

                foreach (var product in data)
                {
                    uniqProductIds.Add(product.Product_Code);

                    if (product.Product_ManagerId > 0)
                    {
                        uniqManagerIds.Add(product.Product_ManagerId);
                    }

                    if (product.Product_ProductTypeId.HasValue)
                    {
                        uniqProductTypeIds.Add(product.Product_ProductTypeId.Value);
                    }

                    if (!string.IsNullOrEmpty(product.Product_Manufacture))
                    {
                        uniqManufacturerIds.Add(product.Product_ManufactureId);
                    }

                    if (!string.IsNullOrEmpty(product.Product_ProductCategory))
                    {
                        var classTitle = product.Product_ProductCategory switch
                        {
                            "Уценка" => "уценка",
                            "О" => "о",
                            "Спец" => "СПЕЦ",
                            _ => product.Product_ProductCategory
                        };

                        uniqClassTitles.Add(classTitle);
                    }

                    if (!string.IsNullOrEmpty(product.Product_Subtype))
                    {
                        uniqSubtypeTitles.Add(product.Product_Subtype);
                    }
                }
                uniqProductIds = uniqProductIds.Distinct().ToList();
                uniqProductTypeIds = uniqProductTypeIds.Distinct().ToList();
                uniqManufacturerIds = uniqManufacturerIds.Distinct().ToList();
                uniqClassTitles = uniqClassTitles.Distinct().ToList();
                uniqSubtypeTitles = uniqSubtypeTitles.Distinct().ToList();
                uniqManagerIds = uniqManagerIds.Distinct().ToList();

                var productMap = await mapDb.ProductMaps.AsNoTracking()
                    .Where(cm => uniqProductIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(cm => cm.LegacyId, cm => cm.ErpGuid, cancellationToken);

                var productTypeMap = await mapDb.ProductTypeMaps.AsNoTracking()
                    .Where(cm => uniqProductTypeIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(cm => cm.LegacyId, cm => cm.ErpGuid, cancellationToken);

                var manufacturerMap = await mapDb.ManufacturerMaps.AsNoTracking()
                    .Where(cm => uniqManufacturerIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(cm => cm.LegacyId, cm => cm.ErpGuid, cancellationToken);

                var classMap = await mapDb.ClassMaps.AsNoTracking()
                    .Where(cm => uniqClassTitles.Contains(cm.LegacyTitle))
                    .ToDictionaryAsync(cm => cm.LegacyTitle, cm => cm.ErpGuid, cancellationToken);

                var subtypeMap = (await mapDb.ProductSubtypeMaps.AsNoTracking()
                    .Where(cm => uniqSubtypeTitles.Contains(cm.Title))
                    .ToListAsync(cancellationToken))
                    .GroupBy(cm => cm.Title)
                    .ToDictionary(cm => cm.Key, cm => new SubtypeData
                    {
                        LegacyId = cm.First().LegacyId,
                        ExternalId = cm.FirstOrDefault()?.ErpGuid
                    });

                var managerMap = await mapDb.EmployeeMaps.AsNoTracking()
                    .Where(cm => uniqManagerIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(cm => cm.LegacyId, cm => cm.ErpGuid, cancellationToken);

                var productTypeCategoryMaps = await mapDb.ProductTypeCategoryMaps
                        .AsNoTracking()
                        .ToDictionaryAsync(e => e.LegacyId, e => e.ErpGuid);
                
                var productTypeCategoryParameterMaps = await mapDb.ProductTypeCategoryParameterMaps
                    .AsNoTracking()
                    .ToDictionaryAsync(e => e.LegacyId, e => e.ErpGuid);

                var parametersMaps = new Dictionary<int, IEnumerable<ProductCategoryParameter>>();
                foreach (var parametersGroup in productParametersGroups)
                {
                    var mapsOfProductParameters = new List<ProductCategoryParameter>();
                    foreach (var productParameter in parametersGroup)
                    {
                        if (productTypeCategoryMaps.ContainsKey(productParameter.CategoryId) && productTypeCategoryParameterMaps.ContainsKey(productParameter.ParameterId))
                        {
                            var productTypeCategoryMap = productTypeCategoryMaps[productParameter.CategoryId];
                            var productTypeCategoryParameterMap = productTypeCategoryParameterMaps[productParameter.ParameterId];

                            if (productTypeCategoryMap.HasValue && productTypeCategoryParameterMap.HasValue)
                            {
                                mapsOfProductParameters.Add(
                                    new ProductCategoryParameter(
                                        new IdMap(productParameter.CategoryId, productTypeCategoryMap),
                                        new IdMap(productParameter.ParameterId, productTypeCategoryParameterMap))

                                );
                            }
                        }
                    }
                    parametersMaps.Add(parametersGroup.Key, mapsOfProductParameters);
                }

                return new Maps
                {
                    ProductMap = productMap,
                    ManufacturerMap = manufacturerMap,
                    ClassMap = classMap,
                    SubtypeMap = subtypeMap,
                    ProductTypeMap = productTypeMap,
                    CountryIsoMap = countryIsoMap,
                    ParametersMap = parametersMaps,
                    ManagerMap = managerMap
                };
            }
        }
    }
}