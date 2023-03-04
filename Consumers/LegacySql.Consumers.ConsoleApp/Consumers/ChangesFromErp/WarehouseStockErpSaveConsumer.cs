using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.WarehouseStocks.Import;
using MessageBus.WarehouseStocks.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class WarehouseStockErpSaveConsumer : BaseConsumer<PublishErpWarehouseStockMessage>
    {
        private readonly IMediator _mediator;

        public WarehouseStockErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpWarehouseStockMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpWarehouseStockDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
