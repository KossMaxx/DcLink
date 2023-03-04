using LegacySql.Commands.Shared;
using LegacySql.Domain.ActivityTypes;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.ActivityTypes.Export;
using MessageBus.ActivityTypes.Export.Add;
using MessageBus.ActivityTypes.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.ActivityTypes.PublishActivityTypes
{
    internal class PublishActivityTypesCommandHandler : ManagedCommandHandler<PublishActivityTypesCommand>
    {
        private readonly IBus _bus;
        private readonly ISagaLogger _sagaLogger;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ILegacyActivityTypesRepository _legacyActivityTypesRepository;
        private readonly IActivityTypesMapRepository _activityTypesMapRepository;

        public PublishActivityTypesCommandHandler(
            ILogger<PublishActivityTypesCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ISagaLogger sagaLogger,
            ILegacyActivityTypesRepository legacyActivityTypesRepository,
            IActivityTypesMapRepository activityTypesMapRepository, 
            ISqlMessageFactory messageFactory) : base(logger, manager)
        {
            _bus = bus;
            _sagaLogger = sagaLogger;
            _legacyActivityTypesRepository = legacyActivityTypesRepository;
            _activityTypesMapRepository = activityTypesMapRepository;
            _messageFactory = messageFactory;
        }

        public override async Task HandleCommand(PublishActivityTypesCommand command, CancellationToken cancellationToken)
        {
            var types = _legacyActivityTypesRepository.GetAllAsync(cancellationToken);
            await foreach (var type in types)
            {
                if (type.IsNew())
                {
                    var dto = new ActivityTypeDto
                    {
                        Code = type.Id.InnerId,
                        Title = type.Title
                    };
                    var message = _messageFactory.CreateNewEntityMessage<AddActivityTypeMessage, ActivityTypeDto>(dto);
                    await _bus.Publish(message, cancellationToken);

                    var mapping = new ExternalMap(message.MessageId, type.Id.InnerId);
                    await _activityTypesMapRepository.SaveAsync(mapping);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.Value.Code);
                }

                if (type.IsChanged())
                {
                    var dto = new ActivityTypeDto
                    {
                        Code = type.Id.InnerId,
                        Title = type.Title
                    };
                    var message = _messageFactory.CreateChangedEntityMessage<ChangeActivityTypeMessage, ActivityTypeDto>(type.Id.ExternalId.Value, dto);
                    await _bus.Publish(message, cancellationToken);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, message.Value.Code);
                }
            }
        }
    }
}
