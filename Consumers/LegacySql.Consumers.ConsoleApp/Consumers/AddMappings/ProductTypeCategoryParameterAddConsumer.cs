using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ProductTypes.AddProductTypeCategoryParameterMap;
using MassTransit;
using MediatR;
using MessageBus.ProductTypes.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ProductTypeCategoryParameterAddConsumer : BaseConsumer<AddedProductTypeCategoryParameterMessage>
    {
        private readonly IMediator _mediator;
        public ProductTypeCategoryParameterAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedProductTypeCategoryParameterMessage> context)
        {
            var message = context.Message;
            var command = new AddProductTypeCategoryParameterMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
