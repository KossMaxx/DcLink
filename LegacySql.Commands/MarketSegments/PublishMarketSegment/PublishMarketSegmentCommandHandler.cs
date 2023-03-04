using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Commands.Shared;
using LegacySql.Domain.MarketSegments;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.MarketSegments.Export;
using MessageBus.MarketSegments.Export.Add;
using MessageBus.MarketSegments.Export.Change;
using Microsoft.Extensions.Logging;
using Sagas.Contracts;

namespace LegacySql.Commands.MarketSegments.PublishMarketSegment
{
    public class PublishMarketSegmentCommandHandler : ManagedCommandHandler<PublishMarketSegmentCommand>
    {
        private readonly ILegacyMarketSegmentRepository _legacyMarketSegmentRepository;
        private readonly IBus _bus;
        private readonly ILastChangedDateRepository _lastChangedDateRepository;
        private readonly IMarketSegmentMapRepository _marketSegmentMapRepository;
        private readonly ISqlMessageFactory _messageFactory;
        private readonly ISagaLogger _sagaLogger;

        public PublishMarketSegmentCommandHandler(
            ILogger<PublishMarketSegmentCommandHandler> logger,
            ICommandsHandlerManager manager,
            ILegacyMarketSegmentRepository legacyMarketSegmentRepository,
            IBus bus, ILastChangedDateRepository lastChangedDateRepository,
            IMarketSegmentMapRepository marketSegmentMapRepository,
            ISqlMessageFactory messageFactory,
            ISagaLogger sagaLogger) : base(logger, manager)
        {
            _legacyMarketSegmentRepository = legacyMarketSegmentRepository;
            _bus = bus;
            _lastChangedDateRepository = lastChangedDateRepository;
            _marketSegmentMapRepository = marketSegmentMapRepository;
            _messageFactory = messageFactory;
            _sagaLogger = sagaLogger;
        }

        public override async Task HandleCommand(PublishMarketSegmentCommand command, CancellationToken cancellationToken)
        {
            foreach (var segment in await _legacyMarketSegmentRepository.GetAllAsync(cancellationToken))
            {
                var segmentDto = MapToDto(segment);
                if (segment.IsChanged())
                {
                    var message = _messageFactory.CreateChangedEntityMessage<ChangeMarketSegmentMessage, MarketSegmentDto>(segment.Id.ExternalId.Value, segmentDto);
                    await _bus.Publish(message, cancellationToken);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, message.ErpId, (int)message.Value.Code);
                }

                if (segment.IsNew())
                {
                    var message = _messageFactory.CreateNewEntityMessage<AddMarketSegmentMessage, MarketSegmentDto>(segmentDto);
                    await _bus.Publish(message);

                    _sagaLogger.Log(message.SagaId, SagaState.Published, (int)message.Value.Code);

                    await _marketSegmentMapRepository.SaveAsync(new ExternalMap(message.MessageId, segment.Id.InnerId));
                }
            }

            await _lastChangedDateRepository.SetAsync(typeof(MarketSegment), DateTime.Now);
        }

        private MarketSegmentDto MapToDto(MarketSegment segment)
        {
            return new MarketSegmentDto
            {
                Code = segment.Id.InnerId,
                Title = segment.Title
            };
        }
    }
}
