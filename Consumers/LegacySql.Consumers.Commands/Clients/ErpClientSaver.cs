using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Departments;
using LegacySql.Domain.Employees;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Warehouses;
using MessageBus.Clients.Import;

namespace LegacySql.Consumers.Commands.Clients
{
    [Obsolete]
    public class ErpClientSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IEmployeeMapRepository _employeeMapRepository;
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private readonly IErpChangedRepository _erpChangedRepository;
        private readonly IDepartmentMapRepository _departmentMapRepository;
        private ExternalMap _clientMapping;
        private ExternalMap _mainManagerMapping;
        private ExternalMap _responsibleManagerMapping;
        private ExternalMap _masterClientMapping;
        private Dictionary<Guid, long> _warehouseAccesses = new Dictionary<Guid, long>();
        private Dictionary<Guid, long> _warehousePriority = new Dictionary<Guid, long>();
        private ErpClientDto _client;
        private ExternalMap _departmentMapping;

        public ErpClientSaver(IDbConnection db, 
            IClientMapRepository clientMapRepository, 
            IEmployeeMapRepository employeeMapRepository, 
            IWarehouseMapRepository warehouseMapRepository, 
            IErpChangedRepository erpChangedRepository, 
            IDepartmentMapRepository departmentMapRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _employeeMapRepository = employeeMapRepository;
            _warehouseMapRepository = warehouseMapRepository;
            _erpChangedRepository = erpChangedRepository;
            _departmentMapRepository = departmentMapRepository;
        }

        public void InitErpObject(ErpClientDto client, ExternalMap clientMapping)
        {
            _client = client;
            _clientMapping = clientMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            if (_client.MasterId.HasValue)
            {
                _masterClientMapping = await _clientMapRepository.GetByErpAsync(_client.MasterId.Value);
                if (_masterClientMapping == null)
                {
                    why.Append($"Маппинг клиента (MasterId) id:{_client.MasterId} не найден\n");
                }
            }

            if (_client.MainManagerId.HasValue)
            {
                _mainManagerMapping = await _employeeMapRepository.GetByErpAsync(_client.MainManagerId.Value);
                if (_mainManagerMapping == null)
                {
                    why.Append($"Маппинг сотрудника (MainManager) id:{_client.MainManagerId} не найден\n");
                }
            }

            if (_client.ResponsibleManagerId.HasValue)
            {
                _responsibleManagerMapping = await _employeeMapRepository.GetByErpAsync(_client.ResponsibleManagerId.Value);
                if (_responsibleManagerMapping == null)
                {
                    why.Append($"Маппинг сотрудника (ResponsibleManager) id:{_client.ResponsibleManagerId} не найден\n");
                }
            }

            if (_client.DepartmentId.HasValue)
            {
                _departmentMapping = await _departmentMapRepository.GetByErpAsync(_client.DepartmentId.Value);
                if (_departmentMapping == null)
                {
                    why.Append($"Маппинг отдела партнера (Department) id:{_client.DepartmentId} не найден\n");
                }
            }

            if (_client.WarehouseAccesses != null && _client.WarehouseAccesses.Any())
            {
                foreach (var wa in _client.WarehouseAccesses)
                {
                    var mapping = await _warehouseMapRepository.GetByErpAsync(wa.WarehouseId);
                    if (mapping == null)
                    {
                        why.Append($"Маппинг склада (WarehouseAccesses) id:{wa.WarehouseId} не найден\n");
                    }
                    else
                    {
                        if (!_warehouseAccesses.ContainsKey(wa.WarehouseId))
                        {
                            _warehouseAccesses.Add(wa.WarehouseId, mapping.LegacyId);
                        }
                    }
                }
            }

            if (_client.WarehousePriorities != null && _client.WarehousePriorities.Any())
            {
                foreach (var wp in _client.WarehousePriorities)
                {
                    var mapping = await _warehouseMapRepository.GetByErpAsync(wp.WarehouseId);
                    if (mapping == null)
                    {
                        why.Append($"Маппинг склада (WarehousePriorities) id:{wp.WarehouseId} не найден\n");
                    }
                    else
                    {
                        if (!_warehousePriority.ContainsKey(wp.WarehouseId))
                        {
                            _warehousePriority.Add(wp.WarehouseId, mapping.LegacyId);
                        }
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

        public async Task Save(Guid messageId)
        {
            if (_clientMapping == null)
            {
                await Create(messageId);
            }
            else
            {
                await Update();
            }
        }

        private async Task Create(Guid messageId)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var insertQuery = @"insert into [dbo].[Клиенты] 
                              ([Название],
                              [manager1],
                              [manager2],
                              [Адрес],
                              [НомерТелефона],
                              [email],
                              [Город],
                              [CityRecipient],
                              [колонка],
                              [department])
                              values(@Title,
                              @MainManagerId,
                              @ResponsibleManagerId,
                              @Address,
                              @Phone,
                              @Email,
                              @City,
                              @CityRecipient,
                              @DefaultPriceColumn,
                              @DepartmentId);
                              select cast(SCOPE_IDENTITY() as int)";

                var mainManagerId = _client.MainManagerId.HasValue
                    ? _mainManagerMapping.LegacyId
                    : (long?)null;
                var responsibleManagerId = _client.ResponsibleManagerId.HasValue
                    ? _responsibleManagerMapping.LegacyId
                    : (long?)null;

                var newClientId = (await _db.QueryAsync<int>(insertQuery, new
                {
                    Title = _client.Title,
                    MainManagerId = mainManagerId,
                    ResponsibleManagerId = responsibleManagerId,
                    Address = _client.Address,
                    Phone = _client.Phone,
                    Email = _client.Email,
                    City = _client.City,
                    CityRecipient = _client.CityRecipient,
                    DefaultPriceColumn = _client.DefaultPriceColumn,
                    DepartmentId = _departmentMapping?.LegacyId
                }, transaction)).FirstOrDefault();

                await InsertWarehousesRelations(_client, transaction, newClientId);

                if (_masterClientMapping != null)
                {
                    await SetMasterId(newClientId, transaction);
                    await UpdateMasterClient(transaction);
                }

                transaction.Commit();

                await _clientMapRepository.SaveAsync(new ExternalMap(messageId, newClientId, _client.Id));
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

        private async Task SetMasterId(int newClientId, IDbTransaction transaction)
        {
            var updateClientSqlQuery = @"update [dbo].[Клиенты]
                                        set [MasterId]=@MasterId
                                        where [КодПоставщика] = @Id";
            await _db.ExecuteAsync(updateClientSqlQuery, new
            {
                Id = newClientId,
                MasterId = _masterClientMapping.LegacyId
            }, transaction);
        }

        private async Task Update()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var updateQuery = @"update [dbo].[Клиенты] 
                                    set
                                    [manager1]=@MainManagerId,
                                    [manager2]=@ResponsibleManagerId,
                                    [Адрес]=@Address,
                                    [НомерТелефона]=@Phone,
                                    [email]=@Email,
                                    [Город]=@City,
                                    [CityRecipient]=@CityRecipient,
                                    [колонка]=@DefaultPriceColumn,
                                    [MasterId]=@MasterId,
                                    [department]=@DepartmentId
                                    where [КодПоставщика] = @Id";

                var mainManagerId = _client.MainManagerId.HasValue
                    ? _mainManagerMapping.LegacyId
                    : (long?)null;
                var responsibleManagerId = _client.ResponsibleManagerId.HasValue
                    ? _responsibleManagerMapping.LegacyId
                    : (long?)null;

                await _db.ExecuteAsync(updateQuery, new
                {
                    Id = _clientMapping.LegacyId,
                    //Title = _client.Title,
                    MainManagerId = mainManagerId,
                    ResponsibleManagerId = responsibleManagerId,
                    Address = _client.Address,
                    Phone = _client.Phone,
                    Email = _client.Email,
                    City = _client.City,
                    CityRecipient = _client.CityRecipient,
                    DefaultPriceColumn = _client.DefaultPriceColumn,
                    MasterId = _masterClientMapping?.LegacyId ?? _clientMapping.LegacyId,
                    DepartmentId = _departmentMapping?.LegacyId
                }, transaction);

                await DeleteWarehouseAccesses(transaction);
                await DeleteWarehousePriority(transaction);
                await InsertWarehousesRelations(_client, transaction);

                transaction.Commit();

                await SaveInErpChange(transaction);
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

        private async Task DeleteWarehousePriority(IDbTransaction transaction)
        {
            var selectIdSqlQuery = @"select [sklad_id] from [dbo].[TBL_Client_Sklad_Reserv_Priority]
                                    where [client_id]=@ClientId";
            var currentWarehousesIds = await _db.QueryAsync<int>(selectIdSqlQuery, new
            {
                ClientId = _clientMapping.LegacyId
            }, transaction);

            var warehousesIdsWithMapping = new List<long>();
            foreach (var id in currentWarehousesIds)
            {
                var mapping = await _warehouseMapRepository.GetByLegacyAsync(id);
                if (mapping != null)
                {
                    warehousesIdsWithMapping.Add(mapping.LegacyId);
                }
            }

            if (warehousesIdsWithMapping.Any())
            {
                var deleteWarehousePrioritySqlQuery = $@"delete from [dbo].[TBL_Client_Sklad_Reserv_Priority]
                                                      where [client_id]=@ClientId and [sklad_id] in ({string.Join(",", warehousesIdsWithMapping.Select(g => $"'{g}'"))})";
                await _db.ExecuteAsync(deleteWarehousePrioritySqlQuery, new
                {
                    ClientId = _clientMapping.LegacyId
                }, transaction);
            }
        }

        private async Task DeleteWarehouseAccesses(IDbTransaction transaction)
        {
            var selectIdSqlQuery = @"select [sklad] from [dbo].[webSkladDopusk]
                                    where [klientID]=@ClientId";
            var currentWarehousesIds = await _db.QueryAsync<int>(selectIdSqlQuery, new
            {
                ClientId = _clientMapping.LegacyId
            }, transaction);

            var warehousesIdsWithMapping = new List<long>();
            foreach (var id in currentWarehousesIds)
            {
                var mapping = await _warehouseMapRepository.GetByLegacyAsync(id);
                if (mapping != null)
                {
                    warehousesIdsWithMapping.Add(mapping.LegacyId);
                }
            }

            if (warehousesIdsWithMapping.Any())
            {
                var deleteWarehouseAccessesSqlQuery = $@"delete from [dbo].[webSkladDopusk]
                                                   where [klientID]=@ClientId and [sklad] in ({string.Join(",", warehousesIdsWithMapping.Select(g => $"'{g}'"))})";

                await _db.ExecuteAsync(deleteWarehouseAccessesSqlQuery, new
                {
                    ClientId = _clientMapping.LegacyId
                }, transaction);
            }
        }

        private async Task InsertWarehousesRelations(ErpClientDto client, IDbTransaction transaction, int? newClientId = null)
        {
            if (_clientMapping == null && !newClientId.HasValue)
            {
                throw new NullReferenceException("При сохранении клиента должно быть заполнено хотябы одно поле (_clientMapping или newClientId)");
            }

            if (client.WarehouseAccesses != null && client.WarehouseAccesses.Any())
            {
                var insertWarehouseAccessesSqlQuery = @"insert into [dbo].[webSkladDopusk]
                                                      ([sklad],[klientID],[price]) 
                                                      values (@WarehouseId,@ClientId,@HasAccess)";
                foreach (var wa in client.WarehouseAccesses)
                {
                    await _db.ExecuteAsync(insertWarehouseAccessesSqlQuery, new
                    {
                        WarehouseId = _warehouseAccesses[wa.WarehouseId],
                        ClientId = _clientMapping?.LegacyId ?? newClientId.Value,
                        HasAccess = wa.HasAccess
                    }, transaction);
                }
            }

            if (client.WarehousePriorities != null && client.WarehousePriorities.Any())
            {
                var insertWarehousePrioritySqlQuery = @"insert into [dbo].[TBL_Client_Sklad_Reserv_Priority]
                                                      ([sklad_id],[client_id],[order_index]) 
                                                      values (@WarehouseId,@ClientId,@Priority)";
                foreach (var wa in client.WarehousePriorities)
                {
                    await _db.ExecuteAsync(insertWarehousePrioritySqlQuery, new
                    {
                        WarehouseId = _warehousePriority[wa.WarehouseId],
                        ClientId = _clientMapping?.LegacyId ?? newClientId.Value,
                        Priority = wa.Priority
                    }, transaction);
                }
            }
        }

        private async Task UpdateMasterClient(IDbTransaction transaction)
        {
            var updateSqlQuery = @"update [dbo].[Клиенты] set modified_at=modified_at
                                 where КодПоставщика=@ClientId";
            await _db.ExecuteAsync(updateSqlQuery, new
            {
                ClientId = _masterClientMapping.LegacyId
            }, transaction);
        }

        private async Task SaveInErpChange(IDbTransaction transaction)
        {
            var selectOrderChangedDateQuery = @"select [modified_at] from [dbo].[Клиенты]
                                                where [КодПоставщика]=@ClientId";
            var clientChangedDate = await _db.QueryFirstOrDefaultAsync<DateTime?>(selectOrderChangedDateQuery, new
            {
                ClientId = _clientMapping.LegacyId
            }, transaction);

            if (clientChangedDate.HasValue)
            {
                await _erpChangedRepository.Save(
                    _clientMapping.LegacyId,
                    clientChangedDate,
                    typeof(Client).Name
                );
            }
        }
    }
}
