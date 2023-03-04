using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Departments;
using LegacySql.Domain.Employees;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.MarketSegments;
using LegacySql.Domain.SegmentationTurnovers;
using LegacySql.Domain.Shared;
using LegacySql.Domain.Warehouses;
using LegacySql.Legacy.Data.Repositories.ConsumerCommandContracts;
using MassTransit;
using MessageBus.Clients.Export;
using MessageBus.Clients.Export.Change;
using MessageBus.Clients.Import;

namespace LegacySql.Consumers.Commands.Clients
{
    public class PartnerSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IEmployeeMapRepository _employeeMapRepository;
        private readonly IWarehouseMapRepository _warehouseMapRepository;
        private readonly IErpChangedRepository _erpChangedRepository;
        private readonly IMarketSegmentMapRepository _marketSegmentMapRepository;
        private readonly IDepartmentMapRepository _departmentMapRepository;
        private readonly ISegmentationTurnoversMapRepository _segmentationTurnoverMapRepository;
        private readonly ILegacyClientRepository _legacyClientRepository;

        private ExternalMap _clientMapping;
        private ExternalMap _mainManagerMapping;
        private ExternalMap _responsibleManagerMapping;
        private ExternalMap _marketSegmentMapping;
        private ExternalMap _masterClientMapping;
        private ExternalMap _departmentMapping;
        private ExternalMap _marketSegmentationTurnoverMapping;
        private Dictionary<Guid, long> _warehouseAccesses = new Dictionary<Guid, long>();
        private Dictionary<Guid, long> _warehousePriority = new Dictionary<Guid, long>();
        private ErpPartnerDto _partner;
        private readonly IPartnerStore _partnerStore;
        private readonly IBus _bus;

        public PartnerSaver(IDbConnection db,
            IClientMapRepository clientMapRepository,
            IEmployeeMapRepository employeeMapRepository,
            IWarehouseMapRepository warehouseMapRepository,
            IErpChangedRepository erpChangedRepository,
            IMarketSegmentMapRepository marketSegmentMapRepository,
            IDepartmentMapRepository departmentMapRepository,
            IPartnerStore partnerStore,
            ISegmentationTurnoversMapRepository segmentationTurnoverMapRepository,
            IBus bus, ILegacyClientRepository legacyClientRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _employeeMapRepository = employeeMapRepository;
            _warehouseMapRepository = warehouseMapRepository;
            _erpChangedRepository = erpChangedRepository;
            _marketSegmentMapRepository = marketSegmentMapRepository;
            _departmentMapRepository = departmentMapRepository;
            _partnerStore = partnerStore;
            _segmentationTurnoverMapRepository = segmentationTurnoverMapRepository;
            _bus = bus;
            _legacyClientRepository = legacyClientRepository;
        }

        public void InitErpObject(ErpPartnerDto partner, ExternalMap clientMapping)
        {
            _partner = partner;
            _clientMapping = clientMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            if (_partner.MasterId.HasValue)
            {
                _masterClientMapping = await _clientMapRepository.GetByErpAsync(_partner.MasterId.Value);
                if (_masterClientMapping == null)
                {
                    why.Append($"Маппинг клиента (MasterId) id:{_partner.MasterId} не найден\n");
                }
            }

            if (_partner.MainManagerId.HasValue)
            {
                _mainManagerMapping = await _employeeMapRepository.GetByErpAsync(_partner.MainManagerId.Value);
                if (_mainManagerMapping == null)
                {
                    why.Append($"Маппинг сотрудника (MainManager) id:{_partner.MainManagerId} не найден\n");
                }
            }

            if (_partner.ResponsibleManagerId.HasValue)
            {
                _responsibleManagerMapping = await _employeeMapRepository.GetByErpAsync(_partner.ResponsibleManagerId.Value);
                if (_responsibleManagerMapping == null)
                {
                    why.Append($"Маппинг сотрудника (ResponsibleManager) id:{_partner.ResponsibleManagerId} не найден\n");
                }
            }

            if (_partner.MarketSegmentId.HasValue)
            {
                _marketSegmentMapping = await _marketSegmentMapRepository.GetByErpAsync(_partner.MarketSegmentId.Value);
                if (_marketSegmentMapping == null)
                {
                    why.Append($"Маппинг группы доступа (MarketSegment) id:{_partner.MarketSegmentId} не найден\n");
                }
            }

            if (_partner.DepartmentId.HasValue)
            {
                _departmentMapping = await _departmentMapRepository.GetByErpAsync(_partner.DepartmentId.Value);
                if (_departmentMapping == null)
                {
                    why.Append($"Маппинг отдела партнера (Department) id:{_partner.DepartmentId} не найден\n");
                }
            }

            if (_partner.MarketSegmentationTurnoverId.HasValue)
            {
                _marketSegmentationTurnoverMapping = await _segmentationTurnoverMapRepository.GetByErpAsync(_partner.MarketSegmentationTurnoverId.Value);
                if (_marketSegmentationTurnoverMapping == null)
                {
                    why.Append($"Маппинг значений оборотов (MarketSegmentationTurnover) id:{_partner.MarketSegmentationTurnoverId} не найден\n");
                }
            }

            if (_partner.WarehouseAccesses != null && _partner.WarehouseAccesses.Any())
            {
                foreach (var wa in _partner.WarehouseAccesses)
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

            if (_partner.WarehousePriorities != null && _partner.WarehousePriorities.Any())
            {
                foreach (var wp in _partner.WarehousePriorities)
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
                var procedure = "dbo.E21_model_modify_partner";
                var mainManagerId = _partner.MainManagerId.HasValue
                    ? _mainManagerMapping.LegacyId
                    : (long?)null;
                var responsibleManagerId = _partner.ResponsibleManagerId.HasValue
                    ? _responsibleManagerMapping.LegacyId
                    : (long?)null;
                var procedureParams = new
                {
                    Partner_Title = _partner.Title,
                    Partner_IsSupplier = _partner.IsSupplier,
                    Partner_IsCustomer = _partner.IsCustomer,
                    Partner_IsCompetitor = _partner.IsCompetitor,
                    Partner_Email = _partner.Email,
                    Partner_BalanceCurrencyId = _partner.BalanceCurrencyId,
                    Partner_MainManagerId = mainManagerId,
                    Partner_ResponsibleManagerId = responsibleManagerId,
                    Partner_CreditDays = _partner.CreditDays,
                    Partner_PriceValidDays = _partner.PriceValidDays,
                    Partner_MarketSegmentId = _marketSegmentMapping?.LegacyId,
                    Partner_Credit = _partner.Credit,
                    Partner_SurchargePercents = _partner.SurchargePercents,
                    Partner_BonusPercents = _partner.BonusPercents,
                    Partner_DelayOk = _partner.DelayOk,
                    Partner_DeliveryTel = _partner.DeliveryTel,
                    Product_Website = _partner.Website,
                    Partner_ContactPerson = _partner.ContactPerson,
                    Partner_ContactPersonPhone = _partner.ContactPersonPhone,
                    Partner_Address = _partner.Address,
                    Partner_MobilePhone = _partner.MobilePhone,
                    Partner_DefaultPriceColumn = _partner.DefaultPriceColumn,
                    Partner_DepartmentId = _departmentMapping?.LegacyId,
                    Partner_City = _partner.City,
                    Partner_SegmentationTurnover = _marketSegmentationTurnoverMapping?.LegacyId,
                    Partner_ScContactPerson = _partner.ScContactPerson ?? "",
                    Partner_ScContactPhone = _partner.ScContactPhone ?? "",
                    Partner_ScContactEmail = _partner.ScContactEmail ?? "",
                    Partner_ScDeliveryAddress = _partner.ScDeliveryAddress ?? "",
                    Partner_ScDeliveryRecipient = _partner.ScDeliveryRecipient ?? "",
                    Partner_ScDeliveryPhone = _partner.ScDeliveryPhone ?? ""
                };
                var newClientId = (await _db.QueryAsync<int>(procedure, procedureParams, transaction, 300, CommandType.StoredProcedure)).FirstOrDefault();

                await InsertWarehousesRelations(transaction, newClientId);
                await InsertDeliveryAddresses(transaction, newClientId);

                if (_masterClientMapping != null)
                {
                    await SetMasterId(newClientId, transaction);
                    await UpdateMasterClient(transaction);
                }

                transaction.Commit();

                await _clientMapRepository.SaveAsync(new ExternalMap(messageId, newClientId, _partner.Id));

                var masterId = _masterClientMapping == null ? newClientId : _masterClientMapping.LegacyId;
                await PublishNewClient(masterId);

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
                var mainManagerId = _partner.MainManagerId.HasValue
                    ? _mainManagerMapping.LegacyId
                    : (long?)null;
                var responsibleManagerId = _partner.ResponsibleManagerId.HasValue
                    ? _responsibleManagerMapping.LegacyId
                    : (long?)null;

                await _partnerStore.Update(
                    _clientMapping.LegacyId,
                    _partner.Title,
                    _partner.IsSupplier,
                    _partner.IsCustomer,
                    _partner.IsCompetitor,
                    _partner.Email,
                    _partner.BalanceCurrencyId,
                    (int)mainManagerId,
                    (int)responsibleManagerId,
                    _partner.CreditDays,
                    _partner.PriceValidDays,
                    _marketSegmentMapping?.LegacyId,
                    _partner.Credit,
                    _partner.SurchargePercents,
                    _partner.BonusPercents,
                    _partner.DelayOk,
                    _partner.DeliveryTel,
                    _partner.Website,
                    _partner.ContactPerson,
                    _partner.ContactPersonPhone,
                    _partner.Address,
                    _partner.MobilePhone,
                    _partner.DefaultPriceColumn,
                    _departmentMapping?.LegacyId,
                    _partner.City,
                    _marketSegmentationTurnoverMapping?.LegacyId,
                    transaction);

                await DeleteWarehouseAccesses(transaction);
                await DeleteWarehousePriority(transaction);
                await DeleteDeliveryAddresses(transaction);
                await InsertWarehousesRelations(transaction);
                await InsertDeliveryAddresses(transaction);

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

        private async Task DeleteDeliveryAddresses(IDbTransaction transaction)
        {
            if (_partner.DeliveryAddresses.Any())
            {
                var deleteSqlQuery = @"delete from [dbo].[TBL_Clients_Shipping_Addr]
                                     where [client_ID]=@ClientId";

                await _db.ExecuteAsync(deleteSqlQuery, new
                {
                    ClientId = _clientMapping.LegacyId
                }, transaction);
            }
        }

        private async Task InsertWarehousesRelations(IDbTransaction transaction, int? newClientId = null)
        {
            if (_clientMapping == null && !newClientId.HasValue)
            {
                throw new NullReferenceException("При сохранении клиента должно быть заполнено хотябы одно поле (_clientMapping или newClientId)");
            }

            if (_partner.WarehouseAccesses != null && _partner.WarehouseAccesses.Any())
            {
                var insertWarehouseAccessesSqlQuery = @"insert into [dbo].[webSkladDopusk]
                                                      ([sklad],[klientID],[price]) 
                                                      values (@WarehouseId,@ClientId,@HasAccess)";
                foreach (var wa in _partner.WarehouseAccesses)
                {
                    await _db.ExecuteAsync(insertWarehouseAccessesSqlQuery, new
                    {
                        WarehouseId = _warehouseAccesses[wa.WarehouseId],
                        ClientId = _clientMapping?.LegacyId ?? newClientId.Value,
                        HasAccess = wa.HasAccess
                    }, transaction);
                }
            }

            if (_partner.WarehousePriorities != null && _partner.WarehousePriorities.Any())
            {
                var insertWarehousePrioritySqlQuery = @"insert into [dbo].[TBL_Client_Sklad_Reserv_Priority]
                                                      ([sklad_id],[client_id],[order_index]) 
                                                      values (@WarehouseId,@ClientId,@Priority)";
                foreach (var wa in _partner.WarehousePriorities)
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
        
        private async Task InsertDeliveryAddresses(IDbTransaction transaction, int? newClientId = null)
        {
            if (_clientMapping == null && !newClientId.HasValue)
            {
                throw new NullReferenceException("При сохранении клиента должно быть заполнено хотябы одно поле (_clientMapping или newClientId)");
            }

            var insertSqlQuery = @"insert into [dbo].[TBL_Clients_Shipping_Addr]
                                 ([dostavkaAdr],
                                 [dostavkaFIO],
                                 [dostavkaTel],
                                 [WayBIll_addr],
                                 [addr_type],
                                 client_ID) 
                                 values 
                                 (@Address,
                                 @ContactPerson,
                                 @Phone,
                                 @WaybillAddress,
                                 @Type,
                                 @ClientId)";
            foreach (var wa in _partner.DeliveryAddresses)
            {
                await _db.ExecuteAsync(insertSqlQuery, new
                {
                    Address = wa.Address,
                    ContactPerson = wa.ContactPerson,
                    Phone = wa.Phone,
                    WaybillAddress = wa.WaybillAddress,
                    Type = wa.Type,
                    ClientId = _clientMapping?.LegacyId ?? newClientId.Value
                }, transaction);
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

        private async Task PublishNewClient(int newClientId)
        {
            var client = await _legacyClientRepository.GetClient(newClientId, CancellationToken.None);
            if(client == null)
            {
                throw new KeyNotFoundException("Клиент не найден");
            }

            var nestedClientsMaps = new Dictionary<int, Guid>();
            foreach (var nestedClient in client.Nested)
            {
                if (nestedClient.HasMap)
                {
                    var nestedClientMap = await _clientMapRepository.GetByLegacyAsync(nestedClient.Id.InnerId);
                    nestedClientsMaps.Add(nestedClient.Id.InnerId, nestedClientMap.MapId);
                }
            }

            var clientMessage = GetChangeLegacyClientMessage(client, Guid.NewGuid(), nestedClientsMaps);
            await _bus.Publish(clientMessage);
        }

        private ChangeLegacyClientMessage GetChangeLegacyClientMessage(Client client, Guid messageId, Dictionary<int, Guid> nestedClientsMaps)
        {
            var message = new ChangeLegacyClientMessage
            {
                MessageId = messageId,
                SagaId = Guid.NewGuid(),
                Value = MapToMessageDto(client),
                ErpId = client.Id.ExternalId,
                NestedMessages = client.Nested != null
                                 ? client.Nested.Select(n => GetChangeLegacyClientMessage(n, nestedClientsMaps[n.Id.InnerId], n.Nested != null ? n.Nested.ToDictionary(nc => nc.Id.InnerId, nc => Guid.NewGuid()) : null))
                                 : new List<ChangeLegacyClientMessage>()
            };

            return message;
        }

        private ClientDto MapToMessageDto(Client client)
        {
            return new ClientDto
            {
                SupplierCode = client.Id.InnerId,
                Title = client.Title,
                OnlySuperReports = client.OnlySuperReports,
                IsSupplier = client.IsSupplier,
                IsCustomer = client.IsCustomer,
                IsCompetitor = client.IsCompetitor,
                Email = client.Email,
                BalanceCurrencyId = client.BalanceCurrencyId,
                MainManagerId = client.MainManagerId?.ExternalId,
                ResponsibleManagerId = client.ResponsibleManagerId?.ExternalId,
                CreditDays = client.CreditDays,
                PriceValidDays = client.PriceValidDays,
                MarketSegmentId = client.MarketSegmentId?.ExternalId,
                MarketSegmentationTurnoverId = client.MarketSegmentationTurnoverId?.ExternalId,
                Credit = client.Credit,
                SurchargePercents = client.SurchargePercents,
                BonusPercents = client.BonusPercents,
                DelayOk = client.DelayOk,
                Firms = client.Firms.Select(i => new FirmDto
                {
                    TaxCode = i.TaxCode,
                    Title = i.Title,
                    LegalAddress = i.LegalAddress,
                    Address = i.Address,
                    Phone = i.Phone,
                    Account = i.Account,
                    BankCode = i.BankCode,
                    BankName = i.BankName,
                    IsNotResident = i.IsNotResident,
                    PayerCode = i.PayerCode,
                    CertificateNumber = i.CertificateNumber,
                    NotVat = i.NotVat,
                    Code = i.Id.InnerId
                }),
                DeliveryTel = client.DeliveryTel,
                SegmentAccessories = client.SegmentAccessories,
                SegmentActiveNet = client.SegmentActiveNet,
                SegmentAv = client.SegmentAv,
                SegmentComponentsPc = client.SegmentComponentsPc,
                SegmentExpendables = client.SegmentExpendables,
                SegmentKbt = client.SegmentKbt,
                SegmentMbt = client.SegmentMbt,
                SegmentMobile = client.SegmentMobile,
                SegmentNotebooks = client.SegmentNotebooks,
                SegmentPassiveNet = client.SegmentPassiveNet,
                SegmentPeriphery = client.SegmentPeriphery,
                SegmentPrint = client.SegmentPrint,
                SegmentReadyPc = client.SegmentReadyPc,
                Consig = client.Consig,
                IsPcAssembler = client.IsPcAssembler,
                SegmentNetSpecifility = client.SegmentNetSpecifility,
                Website = client.Website,
                ContactPerson = client.ContactPerson,
                ContactPersonPhone = client.ContactPersonPhone,
                Address = client.Address,
                MobilePhone = client.MobilePhone,
                DefaultPriceColumn = client.DefaultPriceColumn,
                DepartmentId = client.DepartmentId?.ExternalId,
                City = client.City,
                RegionId = client.RegionId,
                RegionTitle = client.RegionTitle,
                Bonus = client.Bonus,
                Penya = client.Penya,
                DeliveryAddresses = client.DeliveryAddresses.Select(da => new ClientDeliveryAddressDto
                {
                    Id = da.Id,
                    Address = da.Address,
                    ContactPerson = da.ContactPerson,
                    Phone = da.Phone,
                    Type = da.Type,
                    WaybillAddress = da.WaybillAddress
                }).ToList(),
                WarehouseAccesses = client.WarehouseAccesses
                .Where(wa => wa.WarehouseId.ExternalId.HasValue)
                .Select(wa => new ClientWarehouseAccessDto
                {
                    Id = wa.Id,
                    HasAccess = wa.HasAccess,
                    WarehouseId = wa.WarehouseId.ExternalId.Value
                }).ToList(),
                WarehousePriorities = client.WarehousePriorities
                .Where(wp => wp.WarehouseId.ExternalId.HasValue)
                .Select(wp => new ClientWarehousePriorityDto
                {
                    Id = wp.Id,
                    Priority = wp.Priority,
                    WarehouseId = wp.WarehouseId.ExternalId.Value
                }).ToList(),
                ClientProductGroups = client.ClientProductGroups != null ? client.ClientProductGroups.Select(e => new ClientProductGroupDto
                {
                    Id = e.Id.InnerId,
                    GroupId = e.Id.ExternalId.Value
                }) : null,
                ClientActivityTypes = client.ClientActivityTypes != null ? client.ClientActivityTypes.Select(e => new ClientActivityTypeDto
                {
                    Code = e.Id.InnerId,
                    TypeId = e.Id.ExternalId.Value
                }) : null
            };
        }
    }
}