using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.ProductSubtypes.PublishProductSubtypes;
using LegacySql.Commands.Shared;
using LegacySql.Domain.Classes;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Classes.Export;
using MessageBus.Classes.Export.Add;
using MessageBus.ProductSubtypes.Export;
using MessageBus.ProductSubtypes.Export.Add;
using MessageBus.ProductSubtypes.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.Classes.PublishClasses
{
    public class PublishClassesCommandHandler : ManagedCommandHandler<PublishClassesCommand>
    {
        private readonly ILegacyClassRepository _legacyClassRepository;
        private readonly IClassMapRepository _classMapRepository;
        private readonly IBus _bus;
        private readonly ISqlMessageFactory _messageFactory;

        public PublishClassesCommandHandler(
            ICommandsHandlerManager manager,
            IBus bus,
            ILegacyClassRepository legacyClassRepository,
            IClassMapRepository classMapRepository,
            ILogger<PublishClassesCommandHandler> logger,
            ISqlMessageFactory messageFactory) : base(logger, manager)
        {
            _bus = bus;
            _legacyClassRepository = legacyClassRepository;
            _classMapRepository = classMapRepository;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishClassesCommand command, CancellationToken cancellationToken)
        {
            var allMaps = (await _classMapRepository.GetAllMapTitlesAsync()).ToDictionary(e => e);

            IAsyncEnumerable<ProductClass> productClasses;

            productClasses = _legacyClassRepository.GetChangedClassAsync(cancellationToken);

            await foreach (var productClass in productClasses.WithCancellation(cancellationToken))
            {

                var mappingInfo = productClass.IsMappingsFull();

                if (allMaps.ContainsKey(productClass.Title)) continue;

                //if (!mappingInfo.IsMappingFull) continue;

                var classDto = MapToDto(productClass);

                //if (productClass.IsChanged())
                //{
                //    var message = _messageFactory.CreateChangedEntityMessage<ChangeLegacyProductSubtypeMessage, ProductSubtypeDto>(subtype.Id.ExternalId.Value, subtypeDto);
                //    await _bus.Publish(message, cancellationToken);

                //    _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Code);
                //}

                //if (subtype.IsNew())
                //{
                //    var message = _messageFactory.CreateNewEntityMessage<AddProductSubtypeMessage, ProductSubtypeDto>(subtypeDto);
                //    await _bus.Publish(message, cancellationToken);

                //    _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Code);

                //    await _productSubtypeMapRepository.SaveAsync(new ProductSubtypeMap(message.MessageId, subtype.Id.InnerId, subtype.Title));
                //}
                var message = _messageFactory.CreateNewEntityMessage<AddClassMessage, ClassDto>(classDto);
                await _bus.Publish(message, cancellationToken);

                var mapping = new ClassMap(message.MessageId, productClass.Title);
                await _classMapRepository.SaveAsync(mapping);
            }

            //if (!command.Id.HasValue)
            //{
            //    await _lastChangedDateRepository.SetAsync(typeof(ProductSubtype), DateTime.Now);
            //}

        }

        private ClassDto MapToDto(ProductClass productClass)
        {
            var productTypes = (from productType in productClass.ProductTypes where productType.ExternalId.HasValue select productType.ExternalId.Value).ToList();

            return new ClassDto
            {
                Code = productClass.Id.InnerId,
                Title = productClass.Title,
                ProductTypes = productTypes
            };
        }
    }
}
