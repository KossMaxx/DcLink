using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.MarketSegments.Import;
using MessageBus.MarketSegments.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class MarketSegmentErpSaveConsumer : BaseConsumer<PublishErpMarketSegmentMessage>
    {
        private readonly IMediator _mediator;

        public MarketSegmentErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpMarketSegmentMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpMarketSegmentDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
