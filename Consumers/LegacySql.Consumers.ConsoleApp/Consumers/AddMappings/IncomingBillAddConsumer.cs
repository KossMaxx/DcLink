using LegacySql.Consumers.Commands.IncomingBills.AddIncomingBillMap;
using MassTransit;
using MediatR;
using MessageBus.IncomingBills.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class IncomingBillAddConsumer : BaseConsumer<AddedIncomingBillMessage>
    {
        private readonly IMediator _mediator;
        public IncomingBillAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async override Task ConsumeMessage(ConsumeContext<AddedIncomingBillMessage> context)
        {
            var message = context.Message;
            var command = new AddIncomingBillMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
