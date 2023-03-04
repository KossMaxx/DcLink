using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Warehouses.Import;
using MessageBus.Warehouses.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class WarehouseErpSaveConsumer : BaseConsumer<PublishErpWarehouseMessage>
    {
        private readonly IMediator _mediator;

        public WarehouseErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpWarehouseMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpWarehouseDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
