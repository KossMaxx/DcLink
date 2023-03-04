using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ClientOrder.Import;
using MessageBus.ClientOrder.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ClientOrderDeliveryErpSaveConsumer : BaseConsumer<PublishErpClientOrderDeliveryMessage>
    {
        private readonly IMediator _mediator;

        public ClientOrderDeliveryErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpClientOrderDeliveryMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpClientOrderDeliveryDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
