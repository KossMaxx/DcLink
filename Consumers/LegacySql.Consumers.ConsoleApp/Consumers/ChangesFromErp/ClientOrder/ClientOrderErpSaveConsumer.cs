using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ClientOrder.Import;
using MessageBus.ClientOrder.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ClientOrderErpSaveConsumer : BaseConsumer<PublishErpClientOrderMessage>
    {
        private readonly IMediator _mediator;

        public ClientOrderErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpClientOrderMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpClientOrderDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
