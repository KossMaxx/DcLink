using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ActivityTypes.Import;
using MessageBus.ActivityTypes.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ActivityTypeErpSaveConsumer : BaseConsumer<PublishErpActivityTypeMessage>
    {
        private readonly IMediator _mediator;

        public ActivityTypeErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task ConsumeMessage(ConsumeContext<PublishErpActivityTypeMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpActivityTypeDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
