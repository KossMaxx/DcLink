using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Clients;
using MassTransit;
using MediatR;
using MessageBus.Clients.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class PartnerConsumer : BaseConsumer<PublishErpPartnerMessage>
    {
        private readonly IMediator _mediator;

        public PartnerConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpPartnerMessage> context)
        {
            var message = context.Message;
            var command = new SyncMasterBalanceCommand(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
