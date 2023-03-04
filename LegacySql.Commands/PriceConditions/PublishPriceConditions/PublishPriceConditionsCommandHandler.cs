using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.PriceConditions;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.PriceConditions.Export.Add;
using Microsoft.Extensions.Logging;
using PriceConditionDto = MessageBus.PriceConditions.Export.PriceConditionDto;

namespace LegacySql.Commands.PriceConditions.PublishPriceConditions
{
    public class PublishPriceConditionsCommandHandler : ManagedCommandHandler<PublishPriceConditionsCommand>
    {
        private readonly IBus _bus;
        private readonly ILegacyPriceConditionRepository _priceConditionRepository;
        private readonly IPriceConditionMapRepository _priceConditionMapRepository;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishPriceConditionsCommandHandler(
            IBus bus,
            ILegacyPriceConditionRepository priceConditionRepository,
            ILogger<PublishPriceConditionsCommandHandler> logger,
            ICommandsHandlerManager handlerManager, IPriceConditionMapRepository priceConditionMapRepository, 
            ISqlMessageFactory messageFactory) : base(logger, handlerManager)
        {
            _bus = bus;
            _priceConditionRepository = priceConditionRepository;
            _priceConditionMapRepository = priceConditionMapRepository;
            _messageFactory = messageFactory;
        }
        
        public override async Task HandleCommand(PublishPriceConditionsCommand command, CancellationToken cancellationToken)
        {
                await Publish(command, cancellationToken);
        }

        private async Task Publish(PublishPriceConditionsCommand command, CancellationToken cancellationToken)
        {
            var priceConditions = command.Id.HasValue 
                ? GetPriceConditionAsync(command.Id.Value, cancellationToken) 
                : _priceConditionRepository.GetAllAsync(cancellationToken);

            await foreach (var priceCondition in priceConditions)
            {
                var mappingInfo = priceCondition.IsMappingsFull();

                if (mappingInfo.IsMappingFull)
                {
                    var dto = MapToDto(priceCondition);
                    var message = _messageFactory.CreateNewEntityMessage<AddPriceConditionMessage, PriceConditionDto>(dto);
                    await _bus.Publish(message, cancellationToken);
                    var mapping = new ExternalMap(message.MessageId, priceCondition.Id.InnerId);
                    mapping.MapToExternalId(dto.Id);
                    await _priceConditionMapRepository.SaveAsync(mapping);
                }
            }
        }

        private PriceConditionDto MapToDto(PriceCondition item)
        {
            return new PriceConditionDto
            {
                Id = item.Id.ExternalId ?? Guid.NewGuid(),
                Date = item.Date,
                ClientId = item.ClientId?.ExternalId,
                ProductTypeId = item.ProductTypeId?.ExternalId,
                VendorId = item.VendorId?.ExternalId,
                ProductManager = item.ProductManager,
                PriceType = item.PriceType,
                DateTo = item.DateTo,
                Comment = item.Comment,
                Value = item.Value,
                PercentValue = item.PercentValue,
                UpperThresholdPriceType = item.UpperThresholdPriceType,
                DateMilliseconds = item.Date?.Millisecond ?? 0
            };
        }

        private async IAsyncEnumerable<PriceCondition> GetPriceConditionAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var product = await _priceConditionRepository.GetAsync(id, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException("Условия прайсов по типам не найдены");
            }

            yield return product;
        }
    }
}