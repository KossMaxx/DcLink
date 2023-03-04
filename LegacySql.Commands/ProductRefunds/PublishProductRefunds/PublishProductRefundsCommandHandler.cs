using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.ProductRefunds;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.ProductRefunds.Export;
using MessageBus.ProductRefunds.Export.Add;
using MessageBus.ProductRefunds.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;
using ProductRefundDto = MessageBus.ProductRefunds.Export.ProductRefundDto;

namespace LegacySql.Commands.ProductRefunds.PublishProductRefunds
{
    public class PublishProductRefundsCommandHandler : ManagedCommandHandler<PublishProductRefundsCommand>
    {
        private readonly IBus _bus;
        private readonly ILegacyProductRefundRepository _productRefundRepository;
        private readonly IProductRefundMapRepository _productRefundMapRepository;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly INotFullMappedRepository _notFullMappedRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;

        public PublishProductRefundsCommandHandler(
            IBus bus,
            ILegacyProductRefundRepository productRefundRepository,
            IProductRefundMapRepository productRefundMapRepository,
            INotFullMappedRepository notFullMappedRepository,
            ILastChangedDateRepository lastChangedDateRepository,
            ILogger<PublishProductRefundsCommandHandler> logger,
            ICommandsHandlerManager handlerManager,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, handlerManager)
        {
            _bus = bus;
            _productRefundRepository = productRefundRepository;
            _productRefundMapRepository = productRefundMapRepository;
            _notFullMappedRepository = notFullMappedRepository;
            _lastChangedDateRepository = lastChangedDateRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishProductRefundsCommand command, CancellationToken cancellationToken)
        {
                await Publish(command, cancellationToken);
        }

        private async Task Publish(PublishProductRefundsCommand command, CancellationToken cancellationToken)
        {

            var notFullMappingIds = await _notFullMappedRepository.GetIdsAsync(MappingTypes.ProductRefund);
            var notFullMappingsIdsDictionary = notFullMappingIds.ToDictionary(m => m);

            IAsyncEnumerable<ProductRefund> productRefunds = null;
            var lastChangeDates = new List<DateTime?>();
            if (command.Id.HasValue)
            {
                productRefunds = GetProductRefundAsync(command.Id.Value, cancellationToken);
            }
            else
            {
                var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(ProductRefund));
                if (lastChangedDate.HasValue)
                {
                    lastChangeDates.Add(lastChangedDate);
                }

                productRefunds = _productRefundRepository.GetChangedAsync(lastChangedDate, notFullMappingIds, cancellationToken);
            }

            await foreach (var productRefund in productRefunds)
            {
                if (productRefund.ChangedAt.HasValue)
                {
                    lastChangeDates.Add(productRefund.ChangedAt);
                }

                var mappingInfo = productRefund.IsMappingsFull();

                if (!mappingInfo.IsMappingFull && !notFullMappingsIdsDictionary.ContainsKey(productRefund.Id.InnerId))
                {
                    await _notFullMappedRepository.SaveAsync(new NotFullMapped(productRefund.Id.InnerId,
                        MappingTypes.ProductRefund, DateTime.Now, mappingInfo.Why));
                }

                if (mappingInfo.IsMappingFull)
                {
                    var productRefundDto = MapToDto(productRefund);
                    if (productRefund.IsChanged())
                    {

                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyProductRefundMessage, ProductRefundDto>(productRefund.Id.ExternalId.Value, productRefundDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, productRefund.Id.InnerId);
                    }

                    if (productRefund.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddProductRefundMessage, ProductRefundDto>(productRefundDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, productRefund.Id.InnerId);

                        var mapping = new ExternalMap(message.MessageId, productRefund.Id.InnerId);
                        await _productRefundMapRepository.SaveAsync(mapping);
                    }

                    if (notFullMappingsIdsDictionary.ContainsKey(productRefund.Id.InnerId))
                    {
                        await _notFullMappedRepository.RemoveAsync(new NotFullMapped(productRefund.Id.InnerId,
                            MappingTypes.ProductRefund));
                    }
                }
            }

            if (!command.Id.HasValue && lastChangeDates.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(ProductRefund), lastChangeDates.Max().Value);
            }
        }

        private ProductRefundDto MapToDto(ProductRefund productRefund)
        {
            return new ProductRefundDto
            {
                Id = productRefund.Id.ExternalId,
                Date = productRefund.Date,
                ClientId = productRefund.ClientId.ExternalId,
                ClientOrderId = productRefund.ClientOrderId?.ExternalId,
                Items = productRefund.Items.Select(i => new ProductRefundItemDto
                {
                    NomenclatureId = i.ProductId.ExternalId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }),
            };
        }

        private async IAsyncEnumerable<ProductRefund> GetProductRefundAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var product = await _productRefundRepository.GetProductRefundAsync(id, cancellationToken);

            if (product == null)
            {
                throw new KeyNotFoundException("Возврат от клиентов не найден");
            }

            yield return product;
        }
    }
}