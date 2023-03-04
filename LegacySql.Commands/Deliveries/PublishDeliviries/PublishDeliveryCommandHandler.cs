using LegacySql.Commands.Shared;
using LegacySql.Domain.Deliveries;
using LegacySql.Domain.Deliveries.PublishEntity;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Deliveries.Export;
using MessageBus.Deliveries.Export.Add;
using MessageBus.Deliveries.Export.Change;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sagas.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.Deliveries.PublishDeliveries
{
    public class PublishDeliveryCommandHandler : ManagedCommandHandler<PublishDeliveryCommand>
    {
        private readonly IBus _bus;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;
        private readonly ILegacyDeliveryRepository _legacyDeliveryRepository;
        private readonly IDeliveryMapRepository _deliveryMapRepository;
        public PublishDeliveryCommandHandler(
            ILogger<PublishDeliveryCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ILastChangedDateRepository lastChangedDateRepository,
            INotFullMappedRepository notFullMappedRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger,
            ILegacyDeliveryRepository legacyDeliveryRepository, 
            IDeliveryMapRepository deliveryMapRepository) : base(logger, manager)
        {
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
            _legacyDeliveryRepository = legacyDeliveryRepository;
            _deliveryMapRepository = deliveryMapRepository;
        }

        public override async Task HandleCommand(PublishDeliveryCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.Delivery);
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(Delivery));

            IAsyncEnumerable<Delivery> deliveries;
            if (command.Id.HasValue)
            {
                deliveries = GetDeliveryAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                deliveries = _legacyDeliveryRepository.GetChangedDeliveriesAsync(
                    lastChangedDate, notFullMappingIds, cancellationToken);
            }

            var lastDate = lastChangedDate.HasValue
                ? new List<DateTime> { lastChangedDate.Value }
                : new List<DateTime>();
            await foreach (var delivery in deliveries)
            {
                if (delivery.ChangedAt.HasValue || delivery.CreateDate.HasValue)
                {
                    lastDate.Add(delivery.ChangedAt.HasValue ? delivery.ChangedAt.Value : delivery.CreateDate.Value);
                }

                var mappingInfo = delivery.IsMappingsFull();
                if (!mappingInfo.IsMappingFull)
                {
                    if (!notFullMappingsIdsDictionary.ContainsKey(delivery.Id.InnerId))
                    {
                        await _notFullMappedRepository.SaveAsync(new NotFullMapped(delivery.Id.InnerId,
                            MappingTypes.Delivery, DateTime.Now, mappingInfo.Why));
                    }

                    continue;
                }

                if (mappingInfo.IsMappingFull)
                {
                    if (delivery.IsChanged())
                    {
                        var deliveryDto = MapToDto(delivery);
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyDeliveryMessage, DeliveryDto>(delivery.Id.ExternalId.Value, deliveryDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, message.Value.Code, JsonConvert.SerializeObject(deliveryDto));
                    }

                    if (delivery.IsNew())
                    {
                        var deliveryDto = MapToDto(delivery);
                        var message = _messageFactory.CreateNewEntityMessage<AddDeliveryMessage, DeliveryDto>(deliveryDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.Value.Code, JsonConvert.SerializeObject(deliveryDto));

                        var mapping = new ExternalMap(message.MessageId, delivery.Id.InnerId);
                        await _deliveryMapRepository.SaveAsync(mapping);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(delivery.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(delivery.Id.InnerId,
                            MappingTypes.Delivery));
                    }
                }
            }

            if (!command.Id.HasValue && lastDate.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(Delivery), lastDate.Max());
            }
        }

        private DeliveryDto MapToDto(Delivery delivery)
        {
            return new DeliveryDto
            {
                Code = delivery.Id.InnerId,
                CreateDate = delivery.CreateDate,
                DeparturePlanDate = delivery.DeparturePlanDate,
                CreatorUsername = delivery.CreatorUsername,
                TaskDescription = delivery.TaskDescription,
                Address = delivery.Address,
                CarrierTypeName = delivery.CarrierTypeName,
                CargoInvoice = delivery.CargoInvoice,
                StatusId = delivery.StatusId,
                Category = delivery.Category != null ? new DeliveryCategoryDto
                {
                    Id = delivery.Category.Id,
                    Title = delivery.Category.Title
                } : null,
                ReceivedEmployeeId = delivery.ReceivedEmployeeId?.ExternalId,
                IsCompleted = delivery.IsCompleted,
                PerformedDate = delivery.PerformedDate,
                IsFinished = delivery.IsFinished,
                Stickers = delivery.Stickers,
                Warehouses = delivery.Warehouses.Select(e => new DeliveryWarehouseDto
                {
                    ClientOrderId = e.ClientOrderId?.ExternalId,
                    TypeId = e.TypeId
                })
            };
        }

        private async IAsyncEnumerable<Delivery> GetDeliveryAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var order = await _legacyDeliveryRepository.GetDeliveryAsync(id, cancellationToken);
            if (order == null)
            {
                throw new KeyNotFoundException("Доставка заказ не найдена");
            }

            yield return order;
        }
    }
}
