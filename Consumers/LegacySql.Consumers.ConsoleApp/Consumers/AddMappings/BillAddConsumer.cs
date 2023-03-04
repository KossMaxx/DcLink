using LegacySql.Consumers.Commands.Bills.AddBillMap;
using MassTransit;
using MediatR;
using MessageBus.Bills.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class BillAddConsumer : BaseConsumer<AddedBillMessage>
    {
        private readonly IMediator _mediator;
        public BillAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<AddedBillMessage> context)
        {
            var message = context.Message;
            var command = new AddBillMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
