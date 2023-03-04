using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using LegacySql.Consumers.Commands.Penalties.SaveErpPenalty;
using MassTransit;
using MediatR;
using MessageBus.Penalties.Import;
using MessageBus.Penalties.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class PenaltyErpSaveConsumer : BaseConsumer<PublishErpPenaltyMessage>
    {
        private readonly IMediator _mediator;

        public PenaltyErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpPenaltyMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpPenaltyDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
