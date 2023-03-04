using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ClientOrder.Import;
using MessageBus.ClientOrder.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ClientOrderSerialNumbersErpSaveConsumer : BaseConsumer<PublishErpClientOrderSerialNumbersMessage>
    {
        private readonly IMediator _mediator;

        public ClientOrderSerialNumbersErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpClientOrderSerialNumbersMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpClientOrderSerialNumbersDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
