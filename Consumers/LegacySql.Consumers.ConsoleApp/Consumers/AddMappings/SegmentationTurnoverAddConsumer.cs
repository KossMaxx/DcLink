using LegacySql.Consumers.Commands.SegmentationTurnovers.AddSegmentationTurnover;
using MassTransit;
using MediatR;
using MessageBus.SegmentationTurnovers.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class SegmentationTurnoverAddConsumer : BaseConsumer<AddedSegmentationTurnoverMessage>
    {
        private readonly IMediator _mediator;
        public SegmentationTurnoverAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task ConsumeMessage(ConsumeContext<AddedSegmentationTurnoverMessage> context)
        {
            var message = context.Message;
            var command = new AddSegmentationTurnoverCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
