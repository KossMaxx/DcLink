using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Cashboxes.Import;
using MessageBus.Cashboxes.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class CashboxPaymentErpSaveConsumer : BaseConsumer<PublishErpCashboxPaymentMessage>
    {
        private readonly IMediator _mediator;

        public CashboxPaymentErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpCashboxPaymentMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpCashboxPaymentDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
