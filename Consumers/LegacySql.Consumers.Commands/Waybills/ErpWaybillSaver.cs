using Dapper;
using LegacySql.Domain.Rejects;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Warehouses;
using LegacySql.Domain.Waybills;
using MassTransit;
using MessageBus.Waybills.Export;
using MessageBus.Waybills.Export.Change;
using MessageBus.Waybills.Import;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.Waybills
{
    public class ErpWaybillSaver
    {
        private readonly IDbConnection _db;
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private readonly IWaybillMapRepository _waybillMapRepository;
        private readonly IRejectMapRepository _rejectMapRepository;
        private readonly IBus _bus;
        private ErpWaybillDto _waybill;
        private ExternalMap _warehouseMapping;
        private ExternalMap _waybillMapping;
        private Dictionary<Guid, int> _rejectMappings;

        public ErpWaybillSaver(
            IDbConnection db,
            IWarehouseMapRepository warehouseMapRepository,
            IWaybillMapRepository waybillMapRepository,
            IRejectMapRepository rejectMapRepository, 
            IBus bus)
        {
            _db = db;
            _warehouseMapRepository = warehouseMapRepository;
            _waybillMapRepository = waybillMapRepository;
            _rejectMapRepository = rejectMapRepository;
            _bus = bus;
            _rejectMappings = new Dictionary<Guid, int>();
        }

        internal void InitErpObject(ErpWaybillDto waybill, ExternalMap waybillMapping)
        {
            _waybill = waybill;
            _waybillMapping = waybillMapping;
        }

        internal async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            _warehouseMapping = await _warehouseMapRepository.GetByErpAsync(_waybill.WarehouseId);
            if (_warehouseMapping == null)
            {
                why.Append($"Маппинг склада id:{_waybill.WarehouseId} не найден\n");
            }

            foreach(var rejectId in _waybill.Rejects)
            {
                var mapping = await _rejectMapRepository.GetByErpAsync(rejectId);
                if(mapping == null)
                {
                    why.Append($"Маппинг заказ-наряда id:{rejectId} не найден\n");
                    continue;
                }

                _rejectMappings.Add(rejectId, mapping.LegacyId);
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        internal async Task Create(Guid messageId)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var insertQuery = @"insert into [dbo].[brak_DOCS]
                                    ([docs_D],[description],[skladID],[OOO])
                                    values (@Date,@Description,@WarehouseId,@CompanyId);
                                    select cast(SCOPE_IDENTITY() as int)";
                var newWaybillId = (await _db.QueryAsync<int>(insertQuery, new
                {
                    Date = _waybill.Date,
                    Description = _waybill.Description,
                    WarehouseId = _warehouseMapping.LegacyId,
                    CompanyId = await GetCompanyId(_waybill.CompanyOkpo, transaction)
                }, transaction)).FirstOrDefault();

                if (_waybill.Rejects.Any())
                {
                    await SetRelations(newWaybillId, transaction);
                }

                transaction.Commit();

                await _waybillMapRepository.SaveAsync(new ExternalMap(messageId, newWaybillId, _waybill.Id));
                await PublishNewWaybill(newWaybillId);
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

        private async Task PublishNewWaybill(int newWaybillId)
        {
            var waybillDto = new WaybillDto
            {
                Code = newWaybillId,
                Date = _waybill.Date,
                Description = _waybill.Description,
                CompanyOkpo = _waybill.CompanyOkpo,
                WarehouseId = _waybill.WarehouseId,
                Rejects = _waybill.Rejects
            };
            await _bus.Publish(new ChangeLegacyWaybillMessage
            {
                SagaId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                Value = waybillDto,
                ErpId = _waybill.Id
            });
        }

        internal async Task Update()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var insertQuery = @"update [dbo].[brak_DOCS]
                                    set [description]=@Description,[skladID]=@WarehouseId,[OOO]=@CompanyId
                                    where docs_ID=@WaybillId";
                await _db.QueryAsync<int>(insertQuery, new
                {
                    WaybillId = _waybillMapping.LegacyId,
                    Description = _waybill.Description,
                    WarehouseId = _warehouseMapping.LegacyId,
                    CompanyId = await GetCompanyId(_waybill.CompanyOkpo, transaction)
                }, transaction);

                if (_waybill.Rejects.Any())
                {
                    await ChangeRelations(_waybillMapping.LegacyId, transaction);
                }

                transaction.Commit();
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

        private async Task ChangeRelations(int waybillId, IDbTransaction transaction)
        {
            var selectCurrentSqlQuery = @"select Brak_ID from dbo.Brak_Doc
                                       where docs_ID=@WaybillId";
            var currentRelations = await _db.QueryAsync<int>(selectCurrentSqlQuery, new {
                WaybillId = waybillId
            }, transaction);

            var innerRelations = _rejectMappings.Select(e => e.Value);

            var relationForDelete = innerRelations.Except(currentRelations);
            var relationForInsert = currentRelations.Except(innerRelations);

            var deleteSqlQuery = @"delete from dbo.Brak_Doc
                                   where docs_ID=@WaybillId and Brak_ID=@RejectId";
            foreach(var rejectId in relationForDelete)
            {
                await _db.ExecuteAsync(deleteSqlQuery, new
                {
                    WaybillId = waybillId,
                    RejectId = rejectId
                }, transaction);
            }

            var sqlQuery = @"insert into dbo.Brak_Doc 
                            (docs_ID,Brak_ID)
                            values (@WaybillId,@RejectId)";
            foreach (var rejectId in relationForInsert)
            {
                await _db.ExecuteAsync(sqlQuery, new
                {
                    WaybillId = waybillId,
                    RejectId = rejectId
                }, transaction);
            }

        }

        private async Task SetRelations(int newWaybillId, IDbTransaction transaction)
        {
            var sqlQuery = @"insert into dbo.Brak_Doc 
                            (docs_ID,Brak_ID)
                            values (@WaybillId,@RejectId)";
            
            foreach(var rejectId in _waybill.Rejects)
            {
                await _db.ExecuteAsync(sqlQuery, new
                {
                    WaybillId = newWaybillId,
                    RejectId = _rejectMappings[rejectId]
                }, transaction);
            }
        }

        private async Task<int> GetCompanyId(string companyOkpo, IDbTransaction transaction)
        {
            var sqlQuery = @"select ID from dbo.OOO
                             where okpo=@CompanyOkpo";
            var companyId = await _db.QueryFirstOrDefaultAsync<int?>(sqlQuery, new
            {
                CompanyOkpo = companyOkpo
            }, transaction);

            if(!companyId.HasValue)
            {
                throw new KeyNotFoundException($"Компания с ОКПО {companyOkpo} не найдена");
            }

            return companyId.Value;
        }
    }
}
