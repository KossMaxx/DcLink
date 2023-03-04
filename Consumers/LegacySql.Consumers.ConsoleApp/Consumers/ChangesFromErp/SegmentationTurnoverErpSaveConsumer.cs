using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.SegmentationTurnovers.Import;
using MessageBus.SegmentationTurnovers.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class SegmentationTurnoverErpSaveConsumer : BaseConsumer<PublishErpSegmentationTurnoverMessage>
    {
        private readonly IMediator _mediator;

        public SegmentationTurnoverErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task ConsumeMessage(ConsumeContext<PublishErpSegmentationTurnoverMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpSegmentationTurnoverDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
