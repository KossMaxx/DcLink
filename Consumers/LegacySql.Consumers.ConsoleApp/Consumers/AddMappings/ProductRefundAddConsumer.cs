using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ProductRefunds.AddProductRefundMap;
using MassTransit;
using MediatR;
using MessageBus.ProductRefunds.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ProductRefundAddConsumer : BaseConsumer<AddedProductRefundMessage>
    {
        private readonly IMediator _mediator;
        public ProductRefundAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task ConsumeMessage(ConsumeContext<AddedProductRefundMessage> context)
        {
            var message = context.Message;
            var command = new AddProductRefundMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
