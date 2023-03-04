using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.MovementOrders.Import;
using MessageBus.MovementOrders.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class MovementOrderErpSaveConsumer : BaseConsumer<PublishErpMovementOrderMessage>
    {
        private readonly IMediator _mediator;

        public MovementOrderErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<PublishErpMovementOrderMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpMovementOrderDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
