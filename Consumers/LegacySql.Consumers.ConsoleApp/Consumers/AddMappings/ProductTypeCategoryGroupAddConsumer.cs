using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ProductTypeCategoryGroups.AddProductTypeCategoryGroupMap;
using MassTransit;
using MediatR;
using MessageBus.ProductTypeCategoryGroups.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ProductTypeCategoryGroupAddConsumer : BaseConsumer<AddedProductTypeCategoryGroupMessage>
    {
        private readonly IMediator _mediator;
        public ProductTypeCategoryGroupAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedProductTypeCategoryGroupMessage> context)
        {
            var message = context.Message;
            var command = new AddProductTypeCategoryGroupMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
