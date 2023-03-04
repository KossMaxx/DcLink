using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ClientOrders.AddClientOrderMap;
using MassTransit;
using MediatR;
using MessageBus.ClientOrder.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ClientOrderAddConsumer : BaseConsumer<AddedClientOrderMessage>
    {
        private readonly IMediator _mediator;
        public ClientOrderAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task ConsumeMessage(ConsumeContext<AddedClientOrderMessage> context)
        {
            var message = context.Message;
            var command = new AddClientOrderMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
