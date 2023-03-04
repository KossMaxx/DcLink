using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.BankPayments.Import;
using MessageBus.BankPayments.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class BankPaymentErpSaveConsumer : BaseConsumer<PublishErpBankPaymentMessage>
    {
        private readonly IMediator _mediator;

        public BankPaymentErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpBankPaymentMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpBankPaymentDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
