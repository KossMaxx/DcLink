using LegacySql.Commands.Shared;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.ProductMoving;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.ProductMovings.Export;
using MessageBus.ProductMovings.Export.Add;
using MessageBus.ProductMovings.Export.Change;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sagas.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.ProductMovings.PublishProductMovings
{
    public class PublishProductMovingCommandHandler : ManagedCommandHandler<PublishProductMovingCommand>
    {
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyProductMovingRepository _legacyProductMovingRepository;
        private readonly IProductMovingMapRepository _productMovingMapRepository;
        private readonly IBus _bus;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;
        public PublishProductMovingCommandHandler(
            ILogger<PublishProductMovingCommandHandler> logger,
            ICommandsHandlerManager manager,
            ILastChangedDateRepository lastChangedDateRepository,
            IBus bus,
            INotFullMappedRepository notFullMappedRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger,
            IProductMovingMapRepository productMovingMapRepository, 
            ILegacyProductMovingRepository legacyProductMovingRepository) : base(logger, manager)
        {
            _lastChangedDateRepository = lastChangedDateRepository;
            _bus = bus;
            _notFullMappedRepository = notFullMappedRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
            _productMovingMapRepository = productMovingMapRepository;
            _legacyProductMovingRepository = legacyProductMovingRepository;
        }

        public async override Task HandleCommand(PublishProductMovingCommand command, CancellationToken cancellationToken)
        {
            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.ProductMoving);

            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);
            IAsyncEnumerable<ProductMoving> productsMovings;
            var lastChangeDates = new List<DateTime?>();
            if (command.Id.HasValue)
            {
                productsMovings = GetProductAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(ProductMoving));
                if (lastChangedDate.HasValue)
                {
                    lastChangeDates.Add(lastChangedDate);
                }

                productsMovings = _legacyProductMovingRepository.GetChangedProductMovingsAsync(lastChangedDate, notFullMappingIds,
                    cancellationToken);
            }

            await foreach (var productsMoving in productsMovings)
            {
                if (productsMoving == null)
                {
                    return;
                }

                lastChangeDates.Add(productsMoving.ChangedAt);

                var mappingInfo = productsMoving.IsMappingsFull();
                if (!mappingInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(productsMoving.Id.InnerId))
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(productsMoving.Id.InnerId,
                        MappingTypes.ProductMoving, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    var productMovingDto = MapToDto(productsMoving);
                    if (productsMoving.IsChanged())
                    {
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyProductMovingMessage, ProductMovingDto>(productsMoving.Id.ExternalId.Value, productMovingDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Code);
                    }

                    if (productsMoving.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddProductMovingMessage, ProductMovingDto>(productMovingDto);
                        await _bus.Publish(message, cancellationToken);
                        await _productMovingMapRepository.SaveAsync(new ExternalMap(message.MessageId, productsMoving.Id.InnerId));

                        _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Code);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(productsMoving.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(productsMoving.Id.InnerId, MappingTypes.ProductMoving));
                    }
                }
            }

            if (!command.Id.HasValue && lastChangeDates.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(ProductMoving), lastChangeDates.Max().Value);
            }
        }

        private ProductMovingDto MapToDto(ProductMoving productsMoving)
        {
            return new ProductMovingDto
            {
                Code = productsMoving.Id.InnerId,
                Date = productsMoving.Date,
                CreatorUsername = productsMoving.CreatorUsername,
                CreatorId = productsMoving.CreatorId?.ExternalId,
                OutWarehouseId = productsMoving.OutWarehouseId.ExternalId.Value,
                InWarehouseId = productsMoving.InWarehouseId.ExternalId.Value,
                Description = productsMoving.Description,
                Okpo = productsMoving.Okpo,
                IsAccepted = productsMoving.IsAccepted,
                IsForShipment = productsMoving.IsForShipment,
                IsShipped = productsMoving.IsShipped,
                Items = productsMoving.Items.Select(e => new ProductMovingItemDto
                {
                    ProductId = e.ProductId.ExternalId.Value,
                    Amount = e.Amount,
                    Price = e.Price
                })
            };
        }

        private async IAsyncEnumerable<ProductMoving> GetProductAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var product = await _legacyProductMovingRepository.GetProductMovingAsync(id, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException("Трансфер не найден");
            }

            yield return product;
        }
    }
}
