using LegacySql.Commands.Shared;
using LegacySql.Domain.Firms;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Firms.Export;
using MessageBus.Firms.Export.Add;
using MessageBus.Firms.Export.Change;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sagas.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.Firms.PublishFirms
{
    public class PublishFirmsCommandHandler : ManagedCommandHandler<PublishFirmsCommand>
    {
        private readonly IBus _bus;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;
        private readonly IFirmMapRepository _firmMapRepository;
        private readonly ILegacyFirmRepository _legacyFirmRepository;
        public PublishFirmsCommandHandler(
            ILogger<PublishFirmsCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ILastChangedDateRepository lastChangedDateRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger,
            IFirmMapRepository firmMapRepository,
            ILegacyFirmRepository legacyFirmRepository, 
            INotFullMappedRepository notFullMappedRepository) : base(logger, manager)
        {
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
            _firmMapRepository = firmMapRepository;
            _legacyFirmRepository = legacyFirmRepository;
            _notFullMappedRepository = notFullMappedRepository;
        }

        public async override Task HandleCommand(PublishFirmsCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.ClientFirm);
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(Firm));

            IAsyncEnumerable<Firm> firms;
            if (command.Id.HasValue)
            {
                firms = GetFirmAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                firms = _legacyFirmRepository.GetChangedFirmsAsync(
                    lastChangedDate, notFullMappingIds, cancellationToken);
            }

            var lastDate = lastChangedDate.HasValue
                ? new List<DateTime> { lastChangedDate.Value }
                : new List<DateTime>();
            await foreach (var firm in firms)
            {
                if (firm.ChangedAt.HasValue)
                {
                    lastDate.Add(firm.ChangedAt.Value);
                }
                
                var mappingInfo = firm.IsMappingsFull();
                if (!mappingInfo.IsMappingFull)
                {
                    if (!notFullMappingsIdsDictionary.ContainsKey(firm.Id.InnerId))
                    {
                        await _notFullMappedRepository.SaveAsync(new NotFullMapped(firm.Id.InnerId,
                            MappingTypes.ClientFirm, DateTime.Now, mappingInfo.Why));
                    }

                    continue;
                }

                if (mappingInfo.IsMappingFull)
                {
                    if (firm.IsChanged())
                    {
                        var firmDto = MapToDto(firm);
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyFirmMessage, FirmDto>(firm.Id.ExternalId.Value, firmDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, message.Value.Code, JsonConvert.SerializeObject(firmDto));
                    }

                    if (firm.IsNew())
                    {
                        var firmDto = MapToDto(firm);
                        var message = _messageFactory.CreateNewEntityMessage<AddFirmMessage, FirmDto>(firmDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.Value.Code, JsonConvert.SerializeObject(firmDto));

                        var mapping = new ExternalMap(message.MessageId, firm.Id.InnerId);
                        await _firmMapRepository.SaveAsync(mapping);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(firm.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(firm.Id.InnerId,
                            MappingTypes.ClientFirm));
                    }
                }

                if (!command.Id.HasValue && lastDate.Any())
                {
                    await _lastChangedDateRepository.SetAsync(typeof(Firm), lastDate.Max());
                }
            }
        }

        private FirmDto MapToDto(Firm firm)
        {
            return new FirmDto
            {
                Code = firm.Id.InnerId,
                Title = firm.Title,
                TaxCode = firm.TaxCode,
                NotVat = firm.NotVat ?? false,
                ClientId = firm.ClientId?.ExternalId,
                MasterClientId = firm.MasterClientId?.ExternalId,
                Phone = firm.Phone,
                Address = firm.Address,
                LegalAddress = firm.LegalAddress,
                PayerCode = firm.PayerCode,
                CertificateNumber = firm.CertificateNumber,
                Account = firm.Account,
                BankCode = firm.BankCode,
                BankName = firm.BankName,
                IsNotResident = firm.IsNotResident
            };
        }

        private async IAsyncEnumerable<Firm> GetFirmAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var firm = await _legacyFirmRepository.GetFirmAsync(id, cancellationToken);
            if (firm == null)
            {
                throw new KeyNotFoundException("Фирма не найдена");
            }

            yield return firm;
        }
    }
}
