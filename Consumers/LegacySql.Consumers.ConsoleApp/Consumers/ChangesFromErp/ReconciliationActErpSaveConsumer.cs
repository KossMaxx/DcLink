using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ReconciliationActs.Import;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ReconciliationActErpSaveConsumer : BaseConsumer<PublishErpReconciliationActMessage>
    {
        private readonly IMediator _mediator;

        public ReconciliationActErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpReconciliationActMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpReconciliationActDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
