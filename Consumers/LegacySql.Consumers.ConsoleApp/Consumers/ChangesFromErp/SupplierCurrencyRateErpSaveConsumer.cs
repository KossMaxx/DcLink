using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.SupplierCurrencyRates.Import;
using MessageBus.SupplierCurrencyRates.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class SupplierCurrencyRateErpSaveConsumer : BaseConsumer<PublishErpSupplierCurrencyRateMessage>
    {
        private readonly IMediator _mediator;

        public SupplierCurrencyRateErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpSupplierCurrencyRateMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpSupplierCurrencyRateDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
