using LegacySql.Consumers.Commands.Cashboxes.AddCashboxApplicationPaymentMap;
using MassTransit;
using MediatR;
using MessageBus.Cashboxes.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class CashboxApplicationPaymentsAddConsumer : BaseConsumer<AddedCahsboxApplicationPaymentMessage>
    {
        private readonly IMediator _mediator;

        public CashboxApplicationPaymentsAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<AddedCahsboxApplicationPaymentMessage> context)
        {
            var message = context.Message;
            var command = new AddCashboxApplicationPaymentMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
