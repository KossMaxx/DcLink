using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.ProductSubtypes.Export;
using MessageBus.ProductSubtypes.Export.Add;
using MessageBus.ProductSubtypes.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.ProductSubtypes.PublishProductSubtypes
{
    public class PublishProductSubtypesCommandHandler : ManagedCommandHandler<PublishProductSubtypesCommand>
    {
        private readonly IBus _bus;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly ILegacyProductSubtypeRepository _legacyProductSubtypeRepository;
        private readonly IProductSubtypeMapRepository _productSubtypeMapRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;

        public PublishProductSubtypesCommandHandler(
            ILogger<PublishProductSubtypesCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ILastChangedDateRepository lastChangedDateRepository,
            ILegacyProductSubtypeRepository legacyProductSubtypeRepository,
            IProductSubtypeMapRepository productSubtypeMapRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, manager)
        {
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _legacyProductSubtypeRepository = legacyProductSubtypeRepository;
            _productSubtypeMapRepository = productSubtypeMapRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishProductSubtypesCommand command, CancellationToken cancellationToken)
        {
            var lastChangedDate = await _lastChangedDateRepository.GetAsync(typeof(ProductSubtype));
            IAsyncEnumerable<ProductSubtype> subtypes = command.Id.HasValue
                ? GetProductSubtypeAsync(command.Id.Value, cancellationToken)
                : _legacyProductSubtypeRepository.GetChangedProductSubtypesAsync(lastChangedDate, cancellationToken);

            var lastChangeDates = new List<DateTime>();
            if (lastChangedDate.HasValue)
            {
                lastChangeDates.Add(lastChangedDate.Value);
            }

            await foreach (var subtype in subtypes)
            {

                var mappingInfo = subtype.IsMappingsFull();

                if (mappingInfo.IsMappingFull)
                {
                    lastChangeDates.Add(subtype.ChangedAt);

                    var subtypeDto = MapToDto(subtype);
                    if (subtype.IsChanged())
                    {
                        var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyProductSubtypeMessage, ProductSubtypeDto>(subtype.Id.ExternalId.Value, subtypeDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Code);
                    }

                    if (subtype.IsNew())
                    {
                        var message = _messageFactory.CreateNewEntityMessage<AddProductSubtypeMessage, ProductSubtypeDto>(subtypeDto);
                        await _bus.Publish(message, cancellationToken);

                        _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Code);

                        await _productSubtypeMapRepository.SaveAsync(new ProductSubtypeMap(message.MessageId, subtype.Id.InnerId, subtype.Title));
                    }
                }
            }

            if (!command.Id.HasValue && lastChangeDates.Any())
            {
                await _lastChangedDateRepository.SetAsync(typeof(ProductSubtype), lastChangeDates.Max());
            }
        }

        private async IAsyncEnumerable<ProductSubtype> GetProductSubtypeAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var subtype = await _legacyProductSubtypeRepository.GetAsync(id, cancellationToken);

            if (subtype == null)
            {
                throw new KeyNotFoundException("Подтип не найден");
            }

            yield return subtype;
        }

        private ProductSubtypeDto MapToDto(ProductSubtype subtype)
        {
            return new ProductSubtypeDto
            {
                Code = subtype.Id.InnerId,
                Title = subtype.Title,
                ProductTypeId = subtype.ProductTypeId.ExternalId.Value
            };
        }
    }
}
