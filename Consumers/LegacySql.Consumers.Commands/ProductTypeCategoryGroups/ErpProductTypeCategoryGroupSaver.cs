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
using MessageBus.ProductTypeCategoryGroups.Import;
namespace LegacySql.Consumers.Commands.ProductTypeCategoryGroups
{
    public class ErpProductTypeCategoryGroupSaver
    {
        private readonly IDbConnection _db;
        private readonly IProductTypeCategoryGroupMapRepository _productTypeCategoryGroupMapRepository;
        private ProductTypeCategoryGroupErpDto _productTypeCategoryGroup;
        private ProductTypeCategoryGroupMap _productTypeCategoryGroupMapping;


        public ErpProductTypeCategoryGroupSaver(IDbConnection db,
            IProductTypeCategoryGroupMapRepository productTypeCategoryGroupMapRepository)
        {
            _db = db;
            _productTypeCategoryGroupMapRepository = productTypeCategoryGroupMapRepository;
        }

        public void InitErpObject(ProductTypeCategoryGroupErpDto entity, ProductTypeCategoryGroupMap productTypeCategoryGroupMapping)
        {
            _productTypeCategoryGroup = entity;
            _productTypeCategoryGroupMapping = productTypeCategoryGroupMapping;
        }

        public async Task Create(Guid messageId)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var sqlQuery = @"INSERT INTO dbo.TBL_specifications_subcategory (sort, name, name_ua)
                               VALUES (@Sort, @Name, @NameUA);";
                var groupId = (await _db.QueryAsync<int>(sqlQuery, new
                {
                    _productTypeCategoryGroup.Sort,
                    _productTypeCategoryGroup.Name,
                    _productTypeCategoryGroup.NameUA
                }, transaction)).FirstOrDefault();

                transaction.Commit();

                await _productTypeCategoryGroupMapRepository.SaveAsync(
                    new ProductTypeCategoryGroupMap(
                        messageId,
                        groupId,
                        _productTypeCategoryGroup.Id)
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

        public async Task Update(Guid messageId)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var sqlQuery = @"UPDATE dbo.TBL_specifications_subcategory SET sort = @Sort, name = @Name, name_ua = @NameUA WHERE ID = @Id;";
                await _db.ExecuteAsync(sqlQuery, new
                {
                    Id = _productTypeCategoryGroupMapping.LegacyId,
                    _productTypeCategoryGroup.Sort,
                    _productTypeCategoryGroup.Name,
                    _productTypeCategoryGroup.NameUA
                }, transaction);

                transaction.Commit();

                await _productTypeCategoryGroupMapRepository.SaveAsync(
                    new ProductTypeCategoryGroupMap(
                        messageId,
                        _productTypeCategoryGroupMapping.LegacyId,
                        _productTypeCategoryGroup.Id),
                    _productTypeCategoryGroupMapping.Id
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
    }
}
