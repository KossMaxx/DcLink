using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Clients.Import;
using MessageBus.Clients.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ClientFirmErpSaveConsumer : BaseConsumer<PublishErpClientFirmMessage>
    {
        private readonly IMediator _mediator;

        public ClientFirmErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpClientFirmMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpFirmDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
