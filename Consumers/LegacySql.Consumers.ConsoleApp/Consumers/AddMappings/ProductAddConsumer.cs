using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Products.AddProductMap;
using MassTransit;
using MediatR;
using MessageBus.Products.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ProductAddConsumer : BaseConsumer<AddedProductMessage>
    {
        private readonly IMediator _mediator;
        public ProductAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedProductMessage> context)
        {
            var message = context.Message;
            var command = new AddProductMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
