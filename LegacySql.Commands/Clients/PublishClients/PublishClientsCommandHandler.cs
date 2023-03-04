using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.Clients;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Extensions;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Clients.Export;
using MessageBus.Clients.Export.Add;
using MessageBus.Clients.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.Clients.PublishClients
{
    public class PublishClientsCommandHandler : ManagedCommandHandler<PublishClientsCommand>
    {
        private readonly ILegacyClientRepository _legacyClientRepository;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly IBus _bus;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly IErpChangedRepository _erpChangedRepository;
        private readonly ISagaLogger _sagaLogger;

        public PublishClientsCommandHandler(
            ILegacyClientRepository clientRepository,
            IBus bus,
            IClientMapRepository clientMapRepository,
            ILastChangedDateRepository lastChangedDateRepository,
            ILogger<PublishClientsCommandHandler> logger,
            ICommandsHandlerManager handlerManager,
            INotFullMappedRepository notFullMappedRepository,
            IErpChangedRepository erpChangedRepository,
            ISagaLogger sagaLogger) : base(logger, handlerManager)
        {
            _legacyClientRepository = clientRepository;
            _bus = bus;
            _clientMapRepository = clientMapRepository;
            _lastChangedDateRepository = lastChangedDateRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _erpChangedRepository = erpChangedRepository;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishClientsCommand command, CancellationToken cancellationToken)
        {
            var notFullMappings = (await _notFullMappedRepository.GetIdsAsync(MappingTypes.Client))
                .ToDictionary(m => m);

            DateTime? lastDate = null;
            IEnumerable<Client> clients;
            if (command.Id.HasValue)
            {
                var client = await _legacyClientRepository.GetClient(command.Id.Value, cancellationToken);
                if (client != null)
                {
                    clients = new List<Client> { client };
                }
                else
                {
                    throw new KeyNotFoundException("Клиент не найден");
                }
            }
            else
            {
                var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(Client));

                (clients, lastDate) = await _legacyClientRepository.GetChangedClients(
                    lastChangedDate,
                    cancellationToken,
                    notFullMappings.Select(m => m.Key).ToList());
            }

            var erpChangedClients = (await _erpChangedRepository.GetAll(typeof(Client).Name))
                .ToDictionary(e => e.LegacyId, e => e.Date);
            _logger.LogInformation($"Get {clients.Count()} clients");
            foreach (var client in clients)
            {
                if (erpChangedClients.ContainsKey(client.Id.InnerId)
                    && await IsCheckErpChanged(client.Id.InnerId, client.ChangedAt, erpChangedClients))
                {
                    continue;
                }

                var nestedClientsMaps = new Dictionary<int, Guid>();
                foreach (var nestedClient in client.Nested)
                {
                    if (nestedClient.HasMap)
                    {
                        var nestedClientMap = await _clientMapRepository.GetByLegacyAsync(nestedClient.Id.InnerId);
                        nestedClientsMaps.Add(nestedClient.Id.InnerId, nestedClientMap.MapId);
                    }
                    else
                    {
                        nestedClientsMaps.Add(nestedClient.Id.InnerId, Guid.NewGuid());
                    }
                }

                var mappingInfo = client.IsMappingsFull();
                if (!mappingInfo.IsMappingFull)
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(client.Id.InnerId, MappingTypes.Client, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    if (client.IsChanged())
                    {
                        var message = GetChangeLegacyClientMessage(client, Guid.NewGuid(), nestedClientsMaps);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId.Value, (int)message.Value.SupplierCode);

                        var newNestedClientsIds = client.Nested.Where(n => !n.HasMap).Select(n => n.Id.InnerId);
                        foreach (var newNestedClientId in newNestedClientsIds)
                        {
                            await _clientMapRepository.SaveAsync(new ExternalMap(nestedClientsMaps[newNestedClientId], newNestedClientId));
                        }
                    }

                    if (client.IsNew())
                    {
                        var messageId = Guid.NewGuid();
                        var message = GetAddClientMessage(client, messageId, nestedClientsMaps);

                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.SupplierCode);

                        await _clientMapRepository.SaveAsync(new ExternalMap(messageId, client.Id.InnerId));
                        foreach (var key in nestedClientsMaps.Keys)
                        {
                            await _clientMapRepository.SaveAsync(new ExternalMap(nestedClientsMaps[key], key));
                        }
                    }
                    
                    if (notFullMappings.ContainsKey(client.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(client.Id.InnerId, MappingTypes.Client));
                    }
                }
            }

            _logger.LogInformation($"Last change date {lastDate}");
            if (!command.Id.HasValue && lastDate.HasValue)
            {
                await _lastChangedDateRepository.SetAsync(typeof(Client), lastDate.Value);
            }
        }

        private AddClientMessage GetAddClientMessage(Client client, Guid messageId, Dictionary<int, Guid> nestedClientsMaps)
        {
            var message = new AddClientMessage
            {
                MessageId = messageId,
                SagaId = Guid.NewGuid(),
                Value = MapToMessageDto(client),
                NestedMessages = client.Nested != null
                                 ? client.Nested.Select(n => GetAddClientMessage(n, nestedClientsMaps[n.Id.InnerId], n.Nested != null ? n.Nested.ToDictionary(nc => nc.Id.InnerId, nc => Guid.NewGuid()) : null))
                                 : new List<AddClientMessage>()
            };

            return message;
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
                ScContactEmail = client.ScContactEmail,
                ScContactPerson = client.ScContactPerson,
                ScContactPhone = client.ScContactPhone,
                ScDeliveryAddress = client.ScDeliveryAddress,
                ScDeliveryPhone = client.ScDeliveryPhone,
                ScDeliveryRecipient = client.ScDeliveryRecipient,
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

        private async Task<bool> IsCheckErpChanged(int clientId, DateTime? clientChangedAt, Dictionary<int, DateTime> erpChangedOrders)
        {
            var type = GetType();
            var entityName = type.GetEntityName();

            if (erpChangedOrders[clientId] == clientChangedAt)
            {
                _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {type.Name} Id: {clientId} Ignored because changed by ERP");
                return true;
            }

            await _erpChangedRepository.Delete(clientId, typeof(Client).Name);
            _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {type.Name} Id: {clientId} Delete from ErpChanged");

            return false;
        }
    }
}