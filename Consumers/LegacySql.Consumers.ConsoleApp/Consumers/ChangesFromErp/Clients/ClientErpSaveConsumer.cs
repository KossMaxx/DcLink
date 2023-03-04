using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Clients.Import;
using MessageBus.Clients.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ClientErpSaveConsumer : BaseConsumer<PublishErpClientMessage>
    {
        private readonly IMediator _mediator;

        public ClientErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpClientMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpClientDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
