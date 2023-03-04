using LegacySql.Consumers.Commands.ActivityTypes.AddActivityType;
using MassTransit;
using MediatR;
using MessageBus.ActivityTypes.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ActivityTypeAddConsumer : BaseConsumer<AddedActivityTypeMessage>
    {
        private readonly IMediator _mediator;
        public ActivityTypeAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task ConsumeMessage(ConsumeContext<AddedActivityTypeMessage> context)
        {
            var message = context.Message;
            var command = new AddActivityTypeCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
