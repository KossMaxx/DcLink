using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Classes.AddClassMap;
using MassTransit;
using MediatR;
using MessageBus.Classes.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ClassAddConsumer : BaseConsumer<AddedClassMessage>
    {
        private readonly IMediator _mediator;
        public ClassAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedClassMessage> context)
        {
            var message = context.Message;
            var command = new AddClassMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
