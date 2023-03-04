using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.ProductPriceConditions;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.ProductPriceConditions.Export.Add;
using Microsoft.Extensions.Logging;
using ProductPriceConditionDto = MessageBus.ProductPriceConditions.Export.ProductPriceConditionDto;

namespace LegacySql.Commands.ProductPriceConditions.PublishProductPriceConditions
{
    public class PublishProductPriceConditionsCommandHandler : ManagedCommandHandler<PublishProductPriceConditionsCommand>
    {
        private readonly IBus _bus;
        private readonly ILegacyProductPriceConditionRepository _productPriceConditionRepository;
        private readonly IProductPriceConditionMapRepository _productPriceConditionMapRepository;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishProductPriceConditionsCommandHandler(
            IBus bus,
            ILegacyProductPriceConditionRepository productPriceConditionRepository,
            ILogger<PublishProductPriceConditionsCommandHandler> logger,
            ICommandsHandlerManager handlerManager, 
            IProductPriceConditionMapRepository productPriceConditionMapRepository, 
            ISqlMessageFactory messageFactory) : base(logger, handlerManager)
        {
            _bus = bus;
            _productPriceConditionRepository = productPriceConditionRepository;
            _productPriceConditionMapRepository = productPriceConditionMapRepository;
            _messageFactory = messageFactory;
        }
        
        public override async Task HandleCommand(PublishProductPriceConditionsCommand command, CancellationToken cancellationToken)
        {
                await Publish(command, cancellationToken);
        }

        private async Task Publish(PublishProductPriceConditionsCommand command, CancellationToken cancellationToken)
        {
            var productPriceConditions = command.Id.HasValue 
                ? GetProductPriceConditionAsync(command.Id.Value, cancellationToken) 
                : _productPriceConditionRepository.GetAllAsync(cancellationToken);

            await foreach (var productPriceCondition in productPriceConditions)
            {
                var mappingInfo = productPriceCondition.IsMappingsFull();
                
                if (mappingInfo.IsMappingFull)
                {
                    var mapId = Guid.NewGuid();
                    var dto = MapToDto(productPriceCondition);
                    dto.Id = mapId;
                    var message = _messageFactory.CreateNewEntityMessage<AddProductPriceConditionMessage, ProductPriceConditionDto>(dto);
                    await _bus.Publish(message, cancellationToken);

                    var mapping = new ExternalMap(message.MessageId, productPriceCondition.Id.InnerId);
                    mapping.MapToExternalId(mapId);
                    await _productPriceConditionMapRepository.SaveAsync(mapping);
                }
            }
        }

        private ProductPriceConditionDto MapToDto(ProductPriceCondition item)
        {
            return new ProductPriceConditionDto
            {
                ClientId = item.ClientId.ExternalId.Value,
                ProductId = item.ProductId.ExternalId.Value,
                Price = item.Price,
                DateTo = item.DateTo,
                Value = item.Value,
                Currency = item.Currency,
            };
        }

        private async IAsyncEnumerable<ProductPriceCondition> GetProductPriceConditionAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var product = await _productPriceConditionRepository.GetAsync(id, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException("Условия прайсов по товарам не найдены");
            }

            yield return product;
        }
    }
}