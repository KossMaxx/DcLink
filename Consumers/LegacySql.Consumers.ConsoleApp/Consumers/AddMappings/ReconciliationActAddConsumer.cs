using System.Threading.Tasks;
using LegacySql.Consumers.Commands.ReconciliationActs.AddReconciliationActMap;
using MassTransit;
using MediatR;
using MessageBus.ReconciliationActs.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ReconciliationActAddConsumer : BaseConsumer<AddedReconciliationActMessage>
    {
        private readonly IMediator _mediator;
        public ReconciliationActAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedReconciliationActMessage> context)
        {
            var message = context.Message;
            var command = new AddReconciliationActMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
