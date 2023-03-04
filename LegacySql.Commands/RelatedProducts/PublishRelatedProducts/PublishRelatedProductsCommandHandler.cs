using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.RelatedProducts;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.RelatedProducts.Export;
using MessageBus.RelatedProducts.Export.Add;
using Microsoft.Extensions.Logging;

namespace LegacySql.Commands.RelatedProducts.PublishRelatedProducts
{
    public class PublishRelatedProductsCommandHandler : ManagedCommandHandler<PublishRelatedProductsCommand>
    {
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyRelatedProductRepository _legacyRelatedProductRepository;
        private readonly IRelatedProductMapRepository _relatedProductMapRepository;
        private readonly IBus _bus;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishRelatedProductsCommandHandler(
            ILegacyRelatedProductRepository legacyRelatedProductRepository,
            IBus bus, ILastChangedDateRepository lastChangedDateRepository,
            IRelatedProductMapRepository relatedProductMapRepository, 
            INotFullMappedRepository notFullMappedRepository, 
            ICommandsHandlerManager handlerManager, 
            ILogger<PublishRelatedProductsCommandHandler> logger, 
            ISqlMessageFactory messageFactory) : base(logger, handlerManager)
        {
            _legacyRelatedProductRepository = legacyRelatedProductRepository;
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _relatedProductMapRepository = relatedProductMapRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishRelatedProductsCommand command, CancellationToken cancellationToken)
        {
                await Publish(command, cancellationToken);
        }

        private async Task Publish(PublishRelatedProductsCommand command, CancellationToken cancellationToken)
        {
            IAsyncEnumerable<RelatedProduct> relatedProducts;
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.RelatedProduct);
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            var lastChangeDates = new List<DateTime>();

            if (command.Id.HasValue)
            {
                relatedProducts = _legacyRelatedProductRepository.GetRelatedProductAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(RelatedProduct));
                if (lastChangedDate.HasValue)
                {
                    lastChangeDates.Add(lastChangedDate.Value);
                }
                relatedProducts = _legacyRelatedProductRepository.GetChangedAsync(lastChangedDate, notFullMappingIds, cancellationToken);
            }

            await foreach (var relatedProduct in relatedProducts)
            {
                if (relatedProduct.ChangedAt.HasValue)
                {
                    lastChangeDates.Add(relatedProduct.ChangedAt.Value);
                }

                var mappingInfo = relatedProduct.IsMappingsFull();
                if (!mappingInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(relatedProduct.Id))
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(relatedProduct.Id, MappingTypes.RelatedProduct, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    var relatedProductDto = MapToDto(relatedProduct);
                    var message = _messageFactory.CreateNewEntityMessage<AddRelatedProductMessage, RelatedProductDto>(relatedProductDto);
                    await _bus.Publish(message, cancellationToken);
                    await _relatedProductMapRepository.SaveAsync(new ExternalMap(message.MessageId, relatedProduct.Id));

                    if (notFullMappingsIdsDictionary.ContainsKey(relatedProduct.Id))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(relatedProduct.Id, MappingTypes.RelatedProduct));
                    }
                }
            }

            if (!command.Id.HasValue && lastChangeDates.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(RelatedProduct), lastChangeDates.Max());
            }
        }

        private RelatedProductDto MapToDto(RelatedProduct relatedProduct)
        {
            return new RelatedProductDto
            {
                MainProductId = relatedProduct.MainProductId?.ExternalId,
                RelatedProductId = relatedProduct.RelatedProductId?.ExternalId,
            };
        }
    }
}