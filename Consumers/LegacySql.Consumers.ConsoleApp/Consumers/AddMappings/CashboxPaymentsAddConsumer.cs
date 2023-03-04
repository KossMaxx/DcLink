using LegacySql.Consumers.Commands.Cashboxes.AddCashboxPaymentMap;
using MassTransit;
using MediatR;
using MessageBus.Cashboxes.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class CashboxPaymentsAddConsumer : BaseConsumer<AddedCashboxPaymentMessage>
    {
        private readonly IMediator _mediator;

        public CashboxPaymentsAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<AddedCashboxPaymentMessage> context)
        {
            var message = context.Message;
            var command = new AddCashboxPaymentMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
