using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ProductTypes.AddProductTypeMap;
using MassTransit;
using MediatR;
using MessageBus.ProductTypes.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ProductTypeAddConsumer : BaseConsumer<AddedProductTypeMessage>
    {
        private readonly IMediator _mediator;
        public ProductTypeAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedProductTypeMessage> context)
        {
            var message = context.Message;
            var command = new AddProductTypeMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
