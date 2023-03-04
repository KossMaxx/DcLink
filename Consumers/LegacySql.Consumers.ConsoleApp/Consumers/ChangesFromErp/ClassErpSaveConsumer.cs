using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Classes.Import;
using MessageBus.Classes.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ClassErpSaveConsumer : BaseConsumer<PublishErpClassMessage>
    {
        private readonly IMediator _mediator;

        public ClassErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpClassMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpClassDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
