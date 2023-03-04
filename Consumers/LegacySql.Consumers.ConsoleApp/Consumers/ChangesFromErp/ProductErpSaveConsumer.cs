using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Products.Import;
using MessageBus.Products.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ProductErpSaveConsumer : BaseConsumer<PublishErpProductMessage>
    {
        private readonly IMediator _mediator;

        public ProductErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpProductMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpProductDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
