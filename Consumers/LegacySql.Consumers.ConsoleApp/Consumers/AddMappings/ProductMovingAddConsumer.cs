using LegacySql.Consumers.Commands.ProductMovings.AddProductMovingMap;
using MassTransit;
using MediatR;
using MessageBus.ProductMovings.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ProductMovingAddConsumer : BaseConsumer<AddedProductMovingMessage>
    {
        private readonly IMediator _mediator;
        public ProductMovingAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<AddedProductMovingMessage> context)
        {
            var message = context.Message;
            var command = new AddProductMovingMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
