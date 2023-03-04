using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Rejects.Import;
using MessageBus.Rejects.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class RejectErpSaveConsumer : BaseConsumer<PublishErpRejectMessage>
    {
        private readonly IMediator _mediator;

        public RejectErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpRejectMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpRejectDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
