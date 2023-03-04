using System.Threading.Tasks;
using LegacySql.Consumers.Commands.MarketSegments.AddMarketSegmentMap;
using MassTransit;
using MediatR;
using MessageBus.MarketSegments.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class MarketSegmentAddConsumer : BaseConsumer<AddedMarketSegmentMessage>
    {
        private readonly IMediator _mediator;
        public MarketSegmentAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedMarketSegmentMessage> context)
        {
            var message = context.Message;
            var command = new AddMarketSegmentMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
