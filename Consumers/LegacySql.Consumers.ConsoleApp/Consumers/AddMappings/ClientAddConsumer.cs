using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Clients.AddClientMap;
using MassTransit;
using MediatR;
using MessageBus.Clients.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ClientAddConsumer : BaseConsumer<AddedClientMessage>
    {
        private readonly IMediator _mediator;
        public ClientAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedClientMessage> context)
        {
            var message = context.Message;
            var command = new AddClientMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
