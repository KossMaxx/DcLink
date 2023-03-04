using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Purchases.AddPurchaseMap;
using MassTransit;
using MediatR;
using MessageBus.Purchases.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class PurchaseAddConsumer : BaseConsumer<AddedPurchaseMessage>
    {
        private readonly IMediator _mediator;
        public PurchaseAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task ConsumeMessage(ConsumeContext<AddedPurchaseMessage> context)
        {
            var message = context.Message;
            var command = new AddPurchaseMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
