using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ProductTypes.AddProductTypeCategoryMap;
using MassTransit;
using MediatR;
using MessageBus.ProductTypes.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ProductTypeCategoryAddConsumer : BaseConsumer<AddedProductTypeCategoryMessage>
    {
        private readonly IMediator _mediator;
        public ProductTypeCategoryAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task ConsumeMessage(ConsumeContext<AddedProductTypeCategoryMessage> context)
        {
            var message = context.Message;
            var command = new AddProductTypeCategoryMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
