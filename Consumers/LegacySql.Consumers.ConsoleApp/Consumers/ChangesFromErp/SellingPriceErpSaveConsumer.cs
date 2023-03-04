using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.SellingPrices.Import;
using MessageBus.SellingPrices.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class SellingPriceErpSaveConsumer : BaseConsumer<PublishErpSellingPriceMessage>
    {
        private readonly IMediator _mediator;

        public SellingPriceErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpSellingPriceMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpSellingPriceDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
