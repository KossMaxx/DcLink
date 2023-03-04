using LegacySql.Consumers.Commands.Deliveries.AddDeliveryMap;
using MassTransit;
using MediatR;
using MessageBus.Deliveries.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class DeliveryAddConsumer : BaseConsumer<AddedDeliveryMessage>
    {
        private readonly IMediator _mediator;
        public DeliveryAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<AddedDeliveryMessage> context)
        {
            var message = context.Message;
            var command = new AddDeliveryMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
