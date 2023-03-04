using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.ProductTypeCategoryGroups;
using LegacySql.Domain.ProductTypeCategories;
using LegacySql.Domain.ProductTypeCategoryParameters;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using MessageBus.ProductTypes.Import;

namespace LegacySql.Consumers.Commands.ProductTypes
{
    public class ErpProductTypeSaver
    {
        private readonly IDbConnection _db;
        private readonly IProductTypeMapRepository _productTypeMapRepository;
        private readonly IProductTypeCategoryMapRepository _productTypeCategoryMapRepository;
        private readonly IProductTypeCategoryGroupMapRepository _productTypeCategoryGroupMapRepository;
        private readonly IProductTypeCategoryParameterMapRepository _productTypeCategoryParameterMapRepository;
        private ProductTypeMap _productGroupMap;
        private ProductTypeErpDto _productType;
        private ProductTypeMap _productTypeMapping;

        public ErpProductTypeSaver(IDbConnection db,
            IProductTypeMapRepository productTypeMapRepository,
            IProductTypeCategoryMapRepository productTypeCategoryMapRepository,
            IProductTypeCategoryGroupMapRepository productTypeCategoryGroupMapRepository,
            IProductTypeCategoryParameterMapRepository productTypeCategoryParameterMapRepository)
        {
            _db = db;
            _productTypeMapRepository = productTypeMapRepository;
            _productTypeCategoryMapRepository = productTypeCategoryMapRepository;
            _productTypeCategoryGroupMapRepository = productTypeCategoryGroupMapRepository;
            _productTypeCategoryParameterMapRepository = productTypeCategoryParameterMapRepository;
        }

        public async Task<MappingInfo> GetMappingInfo(ProductTypeErpDto type)
        {
            var why = new StringBuilder();
            if (type.MainId.HasValue && type.MainId != Guid.Empty)
            {
                _productGroupMap = await _productTypeMapRepository.GetByErpAsync(type.MainId.Value);
                if (_productGroupMap == null)
                {
                    why.Append($"Отсутствует маппинг группы id: {type.MainId}\n");
                }
            }

            //if (type.Categories.Any())
            //{
            //    foreach (var category in type.Categories)
            //    {
            //        var productTypeCategoryGroupMap =
            //            await _productTypeCategoryGroupMapRepository.GetByErpAsync(category.Group.Id);
            //        if (productTypeCategoryGroupMap == null)
            //        {
            //            why.Append($"Отсутствует маппинг группы категории id: {category.Group.Id}\n");
            //        }
            //    }
            //}

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public void InitErpObject(ProductTypeErpDto entity, ProductTypeMap productTypeMapping)
        {
            _productType = entity;
            _productTypeMapping = productTypeMapping;
        }

        public async Task Create(Guid messageId)
        {
            var sqlQuery = @"INSERT INTO [dbo].[Типы] ([НазваниеТипа],[ПолноеНазв],[web],[TypeNameUkr],[mainID]) 
                               VALUES (@Name, @FullName, @Web, @TypeNameUkr, @MainId); 
                               SELECT CAST(SCOPE_IDENTITY() as int)";
            var productTypeId = (await _db.QueryAsync<int>(sqlQuery, new
            {
                _productType.Name,
                _productType.FullName,
                _productType.Web,
                _productType.TypeNameUkr,
                MainId = _productGroupMap?.LegacyId
            })).FirstOrDefault();

            var newCategoryMappings = new Dictionary<Guid, int>();
            var newParameterMappings = new Dictionary<Guid, int>();
            var newGroupMappings = new Dictionary<Guid, int>();
            if (_productType.Categories.Any())
            {
                var insertCategoriesQuery = @"insert into [dbo].[KategorysByTip]
                                                ([tip],[nazv],[nazv_ua], subID)
                                                values (@TypeId,@Title,@NameUA, @GroupId);
                                                SELECT CAST(SCOPE_IDENTITY() as int)";
                var insertCategoryGroupQuery = @"INSERT INTO dbo.TBL_specifications_subcategory(sort, name, name_ua) 
                                                VALUES (@Sort, @Name, @NameUA);
                                                SELECT CAST(SCOPE_IDENTITY() as int)";

                foreach (var category in _productType.Categories)
                {
                    var categoryGroup = category.Group;
                    int subId;
                    var groupMapping = await _productTypeCategoryGroupMapRepository.GetByErpAsync(categoryGroup.Id);
                    if (groupMapping != null)
                    {
                        subId = groupMapping.LegacyId;
                    }
                    else
                    {
                        subId = (await _db.QueryAsync<int>(insertCategoryGroupQuery, new
                        {
                            categoryGroup.Sort,
                            categoryGroup.Name,
                            categoryGroup.NameUA
                        })).FirstOrDefault();
                        newCategoryMappings.Add(categoryGroup.Id, subId);
                    }

                    var newCategoryId = (await _db.QueryAsync<int>(insertCategoriesQuery, new
                    {
                        TypeId = productTypeId,
                        Title = category.Name,
                        category.NameUA,
                        GroupId = subId,
                    })).FirstOrDefault();

                    newCategoryMappings.Add(category.Id, newCategoryId);


                    if (category.Parameters.Any())
                    {
                        var insertParametersQuery = @"insert into [dbo].[KategoryParameters]
                                                ([katID],[param],[param_ua])
                                                values (@KatId,@Param,@ParamUA);
                                                SELECT CAST(SCOPE_IDENTITY() as int)";
                        foreach (var parameter in category.Parameters)
                        {


                            var newParameterId = (await _db.QueryAsync<int>(insertParametersQuery, new
                            {
                                KatId = newCategoryId,
                                Param = parameter.Name,
                                ParamUA = parameter.NameUA
                            })).FirstOrDefault();

                            newParameterMappings.Add(parameter.Id, newParameterId);
                        }
                    }
                }
            }

            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {

                await _productTypeMapRepository.SaveAsync(
                    new ProductTypeMap(
                        messageId,
                        productTypeId,
                        _productType.Id)
                );

                foreach (var map in newCategoryMappings)
                {
                    await _productTypeCategoryMapRepository.SaveAsync(new ExternalMap(
                        Guid.NewGuid(),
                        map.Value,
                        map.Key
                    ));
                }

                foreach (var map in newGroupMappings)
                {
                    await _productTypeCategoryGroupMapRepository.SaveAsync(new ProductTypeCategoryGroupMap(
                        Guid.NewGuid(),
                        map.Value,
                        map.Key
                    ));
                }

                foreach (var map in newParameterMappings)
                {
                    await _productTypeCategoryParameterMapRepository.SaveAsync(new ExternalMap(
                        Guid.NewGuid(),
                        map.Value,
                        map.Key
                    ));
                }
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

        public async Task Update(Guid messageId)
        {

            const string updateTypeQuery = @"UPDATE [dbo].[Типы] 
                                               SET [НазваниеТипа]=@Name,[ПолноеНазв]=@FullName,[web]=@Web,[TypeNameUkr]=@TypeNameUkr,[mainID]=@MainId 
                                               WHERE [КодТипа]=@Id;";
            const string getCategoriesQuery = @"select * from [dbo].[KategorysByTip]
                                                  where tip=@CategoryId";
            const string insertCategoryQuery = @"insert into [dbo].[KategorysByTip]
                                                   ([tip],[nazv],[nazv_ua],subID)
                                                   values (@TypeId,@Title,@NameUA,@GroupId);
                                                   SELECT CAST(SCOPE_IDENTITY() as int)";
            const string updateCategoryQuery = @"update [dbo].[KategorysByTip]
                                                   set [nazv]=@Title,
                                                   [nazv_ua]=@NameUa,
                                                   subID=@GroupId
                                                   where id=@CategoryId";
            const string deleteCategoryQuery = @"delete from [dbo].[KategorysByTip]
                                                   where id=@CategoryId";
            const string deleteParameterQuery = @"delete from [dbo].[KategoryParameters]
                                                   where id=@ParameterId";
            const string getParametersQuery = @"select * from [dbo].[KategoryParameters]
                                                  where katID=@CategoryId";
            const string insertGroupQuery = @"INSERT INTO dbo.TBL_specifications_subcategory(sort, name, name_ua) 
                                                VALUES (@Sort, @Name, @NameUA);
                                                SELECT CAST(SCOPE_IDENTITY() as int)";
            const string insertParameterQuery = @"insert into [dbo].[KategoryParameters]
                                                    ([katID],[param],[param_ua])
                                                    values (@CategoryId,@Param,@ParamUA);
                                                    SELECT CAST(SCOPE_IDENTITY() as int)";
            const string updateParameterQuery = @"update [dbo].[KategoryParameters]
                                                    set [param]=@Title,
                                                    [param_ua]=@NameUA  
                                                    where id=@ParameterId";
            await _db.ExecuteAsync(updateTypeQuery, new
            {
                Id = _productTypeMapping.LegacyId,
                _productType.Name,
                _productType.FullName,
                _productType.Web,
                _productType.TypeNameUkr,
                MainId = _productGroupMap?.LegacyId
            });

            var categoriesEf = (await _db.QueryAsync(getCategoriesQuery, new
            {
                CategoryId = _productTypeMapping.LegacyId
            }))
                .Select(e => new ProductTypeCategoryEF
                {
                    Id = (int)e.id,
                    Name = (string)e.nazv,
                    NameUA = (string)e.nazv_ua
                }).ToList();

            var parameterEfList = new List<ProductTypeCategoryParameterEF>();

            foreach (var category in categoriesEf)
            {
                var parameterEf = (await _db.QueryAsync(getParametersQuery, new
                {
                    CategoryId = category.Id
                }))
                    .Select(e => new ProductTypeCategoryParameterEF
                    {
                        Id = (int)e.id,
                        Name = (string)e.nazv,
                        NameUA = (string)e.nazv_ua
                    }).ToList();
                parameterEfList.AddRange(parameterEf);
            }

            var legacyCategoryMappings = await _productTypeCategoryMapRepository.GetAllFullMappingsAsync();
            var categoryMappings = legacyCategoryMappings
                .Where(cm => cm.ExternalMapId.HasValue && categoriesEf.Any(ce => ce.Id == cm.LegacyId))
                .Select(em => em.ExternalMapId.Value);
            var legacyParameterMappings = await _productTypeCategoryParameterMapRepository.GetAllFullMappingsAsync();
            var parameterMappings = legacyParameterMappings.
                Where(cm => cm.ExternalMapId.HasValue && parameterEfList.Any(ce => ce.Id == cm.LegacyId)).
                Select(em => em.ExternalMapId.Value);
            var categoryList = _productType.Categories.Select(pc => pc.Id);
            var parameterList = new List<Guid>();
            foreach (var category in _productType.Categories)
            {
                parameterList.AddRange(category.Parameters.Select(cp => cp.Id));
            }
            var categoryListForDelete = categoryMappings.Where(p => categoryList.All(p2 => p2 != p));
            var parameterListForDelete = parameterMappings.Where(p => parameterList.All(p2 => p2 != p));

            var newCategoryMappings = new Dictionary<int, Guid>();
            var newParameterMappings = new Dictionary<int, Guid>();
            var newGroupMappings = new Dictionary<int, Guid>();

            foreach (var category in _productType.Categories)
            {
                var legacyCategory = legacyCategoryMappings.FirstOrDefault(cm => cm.ExternalMapId == category.Id);
                var categoryGroup = category.Group;
                int subId;
                var groupMapping = await _productTypeCategoryGroupMapRepository.GetByErpAsync(categoryGroup.Id);

                if (groupMapping != null)
                {
                    subId = groupMapping.LegacyId;
                }
                else
                {
                    var check = (await _db.QueryAsync<int>(insertGroupQuery, new
                    {
                        categoryGroup.Sort,
                        categoryGroup.Name,
                        categoryGroup.NameUA
                    })).FirstOrDefault();
                    subId = check;
                    newGroupMappings.Add(subId, categoryGroup.Id);
                }

                if (legacyCategory == null)
                {
                    var newCategoryId = (await _db.QueryAsync<int>(insertCategoryQuery, new
                    {
                        TypeId = _productTypeMapping.LegacyId,
                        Title = category.Name,
                        category.NameUA,
                        GroupId = subId
                    })).FirstOrDefault();
                    newCategoryMappings.Add(newCategoryId, category.Id);

                    foreach (var parameter in category.Parameters)
                    {
                        var legacyParameter = legacyParameterMappings.FirstOrDefault(cm => cm.ExternalMapId == parameter.Id);
                        if (legacyParameter == null)
                        {
                            var newParameterId = (await _db.QueryAsync<int>(insertParameterQuery, new
                            {
                                CategoryId = newCategoryId,
                                Param = parameter.Name,
                                ParamUA = parameter.NameUA
                            })).FirstOrDefault();

                            newParameterMappings.Add(newParameterId, parameter.Id);
                        }
                        else
                        {
                            await _db.ExecuteAsync(updateParameterQuery, new
                            {
                                ParameterId = legacyParameter.LegacyId,
                                Title = category.Name,
                                NameUa = category.NameUA
                            });
                        }
                    }
                }
                else
                {
                    await _db.ExecuteAsync(updateCategoryQuery, new
                    {
                        CategoryId = legacyCategory.LegacyId,
                        Title = category.Name,
                        NameUa = category.NameUA,
                        GroupId = subId
                    });

                    foreach (var parameter in category.Parameters)
                    {
                        var legacyParameter = legacyParameterMappings.FirstOrDefault(cm => cm.ExternalMapId == parameter.Id);
                        if (legacyParameter == null)
                        {
                            var newParameterId = (await _db.QueryAsync<int>(insertParameterQuery, new
                            {
                                CategoryId = legacyCategory.LegacyId,
                                Param = parameter.Name,
                                ParamUA = parameter.NameUA
                            })).FirstOrDefault();

                            newParameterMappings.Add(newParameterId, parameter.Id);
                        }
                        else
                        {
                            await _db.ExecuteAsync(updateParameterQuery, new
                            {
                                ParameterId = legacyParameter.LegacyId,
                                Title = category.Name,
                                NameUa = category.NameUA
                            });
                        }
                    }
                }
            }

            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                await _productTypeMapRepository.SaveAsync(
                new ProductTypeMap(
                    messageId,
                    _productTypeMapping.LegacyId,
                    _productType.Id),
                _productTypeMapping.Id
            );

                foreach (var category in newCategoryMappings)
                {
                    await _productTypeCategoryMapRepository.SaveAsync(new ExternalMap(
                        Guid.NewGuid(),
                        category.Key,
                        category.Value)
                    );
                }

                foreach (var group in newGroupMappings)
                {
                    await _productTypeCategoryGroupMapRepository.SaveAsync(new ProductTypeCategoryGroupMap(
                        Guid.NewGuid(),
                        group.Key,
                        group.Value)
                    );
                }


                foreach (var parameter in newParameterMappings)
                {
                    await _productTypeCategoryParameterMapRepository.SaveAsync(new ExternalMap(
                        Guid.NewGuid(),
                        parameter.Key,
                        parameter.Value)
                    );
                }

                foreach (var category in categoryListForDelete)
                {
                    var categoryForDelete = legacyCategoryMappings.FirstOrDefault(cm => cm.ExternalMapId == category);
                    if (categoryForDelete != null)
                    {
                        await _db.QueryAsync<int>(deleteCategoryQuery, new
                        {
                            CategoryId = categoryForDelete.LegacyId
                        }, transaction);
                        await _productTypeCategoryMapRepository.DeleteByIdAsync(categoryForDelete.Id);
                    }
                }



                foreach (var parameter in parameterListForDelete)
                {
                    var parameterForDelete = legacyParameterMappings.FirstOrDefault(cm => cm.ExternalMapId == parameter);
                    if (parameterForDelete != null)
                    {
                        await _db.QueryAsync<int>(deleteParameterQuery, new
                        {
                            ParameterId = parameterForDelete.LegacyId
                        }, transaction);
                        await _productTypeCategoryParameterMapRepository.DeleteByIdAsync(parameterForDelete.Id);
                    }
                }

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
    }
}
