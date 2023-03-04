using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.BankPayments.Import;
using MessageBus.BankPayments.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class PaymentOrderErpSaveConsumer : BaseConsumer<PublishErpPaymentOrderMessage>
    {
        private readonly IMediator _mediator;

        public PaymentOrderErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<PublishErpPaymentOrderMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpPaymentOrderDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
