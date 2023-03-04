using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ProductRefunds.Import;
using MessageBus.ProductRefunds.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ProductRefundErpSaveConsumer : BaseConsumer<PublishErpProductRefundMessage>
    {
        private readonly IMediator _mediator;

        public ProductRefundErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpProductRefundMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpProductRefundDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
