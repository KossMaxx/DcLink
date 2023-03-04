using LegacySql.Commands.Shared;
using LegacySql.Domain.IncomingBills;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.IncomingBills.Export;
using MessageBus.IncomingBills.Export.Add;
using MessageBus.IncomingBills.Export.Change;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sagas.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.Bills.PublishIncomingBills
{
    public class PublishIncomingBillCommandHandler : ManagedCommandHandler<PublishIncomingBillCommand>
    {
        private readonly IBus _bus;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;
        private readonly IIncomingBillMapRepository _incomingBillMapRepository;
        private readonly ILegacyIncomingBillRepository _incomingLegacyBillRepository;
        public PublishIncomingBillCommandHandler(
            ILogger<PublishIncomingBillCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ILastChangedDateRepository lastChangedDateRepository,
            INotFullMappedRepository notFullMappedRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger,
            IIncomingBillMapRepository incomingBillMapRepository, 
            ILegacyIncomingBillRepository incomingLegacyBillRepository) : base(logger, manager)
        {
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
            _incomingBillMapRepository = incomingBillMapRepository;
            _incomingLegacyBillRepository = incomingLegacyBillRepository;
        }

        public async override Task HandleCommand(PublishIncomingBillCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.IncomingBill);
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(IncomingBill));

            IAsyncEnumerable<IncomingBill> bills;
            if (command.Id.HasValue)
            {
                bills = _incomingLegacyBillRepository.GetIncomingBillAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                bills = _incomingLegacyBillRepository.GetChangedIncomingBillOrdersAsync(
                    lastChangedDate, notFullMappingIds, cancellationToken);
            }

            var lastDate = lastChangedDate.HasValue
                ? new List<DateTime> { lastChangedDate.Value }
                : new List<DateTime>();
            await foreach (var bill in bills)
            {
                if (bill.ChangedAt.HasValue)
                {
                    lastDate.Add(bill.ChangedAt.Value);
                }

                var mappingInfo = bill.IsMappingsFull();
                if (!mappingInfo.IsMappingFull)
                {
                    if (!notFullMappingsIdsDictionary.ContainsKey(bill.Id.InnerId))
                    {
                        await _notFullMappedRepository.SaveAsync(new NotFullMapped(bill.Id.InnerId,
                            MappingTypes.IncomingBill, DateTime.Now, mappingInfo.Why));
                    }

                    continue;
                }

                if (mappingInfo.IsMappingFull)
                {
                    if (bill.IsChanged())
                    {
                        var billDto = MapToDto(bill);
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyIncomingBillMessage, IncomingBillDto>(bill.Id.ExternalId.Value, billDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Code, JsonConvert.SerializeObject(billDto));
                    }

                    if (bill.IsNew())
                    {
                        var billDto = MapToDto(bill);
                        var message = _messageFactory.CreateNewEntityMessage<AddIncomingBillMessage, IncomingBillDto>(billDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Code, JsonConvert.SerializeObject(billDto));

                        var mapping = new ExternalMap(message.MessageId, bill.Id.InnerId);
                        await _incomingBillMapRepository.SaveAsync(mapping);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(bill.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(bill.Id.InnerId,
                            MappingTypes.IncomingBill));
                    }
                }
            }

            if (!command.Id.HasValue && lastDate.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(IncomingBill), lastDate.Max());
            }
        }

        private IncomingBillDto MapToDto(IncomingBill bill)
        {
            return new IncomingBillDto
            {
                Code = bill.Id.InnerId,
                Date = bill.Date,
                IncominNumber = bill.IncominNumber,
                RecipientOkpo = bill.RecipientOkpo,
                SupplierOkpo = bill.SupplierOkpo,
                SupplierSqlId = bill.SupplierSqlId,
                ClientId = bill.ClientId != null ? bill.ClientId.ExternalId.Value : null,
                PurchaseId = bill.PurchaseId?.ExternalId,
                Items = bill.Items.Select(e=> new IncomingBillItemDto
                {
                    NomenclatureId = e.NomenclatureId.ExternalId.Value,
                    PriceUAH = e.PriceUAH,
                    Quantity = e.Quantity
                })
            };
        }
    }
}
