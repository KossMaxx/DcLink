using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.Shared;
using MessageBus.Departments.Import;
using MessageBus.Manufacturer.Import;

namespace LegacySql.Consumers.Commands.Manufacturers
{
    public class ErpManufacturerSaver
    {
        private readonly IDbConnection _db;
        private readonly IManufacturerMapRepository _manufacturerMapRepository;
        private ErpManufacturerDto _manufacturer;
        private ManufacturerMap _manufacturerMapping;

        public ErpManufacturerSaver(IDbConnection db,
            IManufacturerMapRepository manufacturerMapRepository)
        {
            _db = db;
            _manufacturerMapRepository = manufacturerMapRepository;
        }

        public void InitErpObject(ErpManufacturerDto manufacturer, ManufacturerMap manufacturerMapping)
        {
            _manufacturer = manufacturer;
            _manufacturerMapping = manufacturerMapping;
        }

        public async Task Save(Guid messageId)
        {
            if (_manufacturerMapping == null)
            {
                await Create(messageId);
            }
            else
            {
                await Update();
            }

            //if (map == null)
            //{
            //    await _manufacturerMapRepository.SaveAsync(
            //        new ManufacturerMap(
            //            command.MessageId,
            //            manufacturer.Code,
            //            map.LegacyId,
            //            manufacturer.Id)
            //    );
            //}
            //else
            //{
            //    await Update(manufacturer, map);
            //}
        }

        private async Task Create(Guid messageId)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var insertSqlQuery = @"execute dbo.E21_model_modify_product_brands NULL, @Product_Manufacture, @Product_ManufactureSite";
            var newManufacturerId = await _db.QueryFirstOrDefaultAsync<int>(insertSqlQuery, new
            {
                Product_Manufacture = _manufacturer.Title,
                Product_ManufactureSite = _manufacturer.Url
            }, transaction);
            transaction.Commit();
                await _manufacturerMapRepository.SaveAsync(new ManufacturerMap(messageId, newManufacturerId, _manufacturer.Title, _manufacturer.Id));
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

        private async Task Update()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var updateSqlQuery = @"execute dbo.E21_model_modify_product_brands @Product_ManufactureId, @Product_Manufacture, @Product_ManufactureSite";
            await _db.ExecuteAsync(updateSqlQuery, new
            {
                Product_ManufactureId = _manufacturerMapping.LegacyId,
                Product_Manufacture = _manufacturer.Title,
                Product_ManufactureSite = _manufacturer.Url
            });

                transaction.Commit();
                await _manufacturerMapRepository.SaveAsync(new ManufacturerMap(_manufacturerMapping.MapId, _manufacturerMapping.LegacyId, _manufacturer.Title, _manufacturerMapping.ExternalMapId, _manufacturerMapping.Id), _manufacturerMapping.Id);
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
