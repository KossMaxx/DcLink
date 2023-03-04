using LegacySql.Commands.Shared;
using LegacySql.Domain.SegmentationTurnovers;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.SegmentationTurnovers.Export;
using MessageBus.SegmentationTurnovers.Export.Add;
using MessageBus.SegmentationTurnovers.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.SegmentationTurnovers
{
    internal class PublishSegmentationTurnoversCommandHandler : ManagedCommandHandler<PublishSegmentationTurnoversCommand>
    {
        private readonly IBus _bus;
        private readonly ISagaLogger _sagaLogger;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ILegacySegmentationTurnoversRepository _legacySegmentationTurnoversRepository;
        private readonly ISegmentationTurnoversMapRepository _segmentationTurnoversMapRepository;

        public PublishSegmentationTurnoversCommandHandler(
            ILogger<PublishSegmentationTurnoversCommandHandler> logger,
            ICommandsHandlerManager manager,
            IBus bus,
            ISagaLogger sagaLogger,
            ISqlMessageFactory messageFactory,
            ISegmentationTurnoversMapRepository segmentationTurnoversMapRepository, 
            ILegacySegmentationTurnoversRepository legacySegmentationTurnoversRepository)
            : base(logger, manager)
        {
            _bus = bus;
            _sagaLogger = sagaLogger;
            _messageFactory = messageFactory;
            _segmentationTurnoversMapRepository = segmentationTurnoversMapRepository;
            _legacySegmentationTurnoversRepository = legacySegmentationTurnoversRepository;
        }

        public override async Task HandleCommand(PublishSegmentationTurnoversCommand command, CancellationToken cancellationToken)
        {
            var turnovers = _legacySegmentationTurnoversRepository.GetAllAsync(cancellationToken);
            await foreach (var turnover in turnovers)
            {
                if (turnover.IsNew())
                {
                    var dto = new SegmentationTurnoverDto
                    {
                        Code = turnover.Id.InnerId,
                        Title = turnover.Title
                    };
                    var message = _messageFactory.CreateNewEntityMessage<AddSegmentationTurnoverMessage, SegmentationTurnoverDto>(dto);
                    await _bus.Publish(message, cancellationToken);

                    var mapping = new ExternalMap(message.MessageId, turnover.Id.InnerId);
                    await _segmentationTurnoversMapRepository.SaveAsync(mapping);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.Value.Code);
                }

                if (turnover.IsChanged())
                {
                    var dto = new SegmentationTurnoverDto
                    {
                        Code = turnover.Id.InnerId,
                        Title = turnover.Title
                    };
                    var message = _messageFactory.CreateChangedEntityMessage<ChangeSegmentationTurnoverMessage, SegmentationTurnoverDto>(turnover.Id.ExternalId.Value, dto);
                    await _bus.Publish(message, cancellationToken);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, message.Value.Code);
                }
            }
        }
    }
}
