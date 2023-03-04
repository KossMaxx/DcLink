using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ProductSubtypes.AddProductSubtypeMap;
using MassTransit;
using MediatR;
using MessageBus.ProductSubtypes.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ProductSubtypeAddConsumer : BaseConsumer<AddedProductSubtypeMessage>
    {
        private readonly IMediator _mediator;
        public ProductSubtypeAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedProductSubtypeMessage> context)
        {
            var message = context.Message;
            var command = new AddProductSubtypeMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
