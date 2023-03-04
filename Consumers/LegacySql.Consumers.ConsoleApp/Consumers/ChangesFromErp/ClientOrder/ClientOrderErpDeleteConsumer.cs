using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ClientOrder.Import;
using MessageBus.ClientOrder.Import.Delete;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ClientOrderErpDeleteConsumer : BaseConsumer<DeleteErpClientOrderMessage>
    {
        private readonly IMediator _mediator;

        public ClientOrderErpDeleteConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<DeleteErpClientOrderMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpClientOrderDeleteIdentifierDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
