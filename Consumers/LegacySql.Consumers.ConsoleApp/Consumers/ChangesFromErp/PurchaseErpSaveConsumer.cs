using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Purchases.Import;
using MessageBus.Purchases.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class PurchaseErpSaveConsumer : BaseConsumer<PublishErpPurchaseMessage>
    {
        private readonly IMediator _mediator;

        public PurchaseErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpPurchaseMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpPurchaseDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
