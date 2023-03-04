using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Rejects.AddRejectMap;
using MassTransit;
using MediatR;
using MessageBus.Rejects.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class RejectAddConsumer : BaseConsumer<AddedRejectMessage>
    {
        private readonly IMediator _mediator;
        public RejectAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedRejectMessage> context)
        {
            var message = context.Message;
            var command = new AddRejectMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
