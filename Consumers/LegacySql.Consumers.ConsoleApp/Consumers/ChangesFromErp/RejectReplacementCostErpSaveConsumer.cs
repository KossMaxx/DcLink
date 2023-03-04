using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Rejects.Import;
using MessageBus.Rejects.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class RejectReplacementCostErpSaveConsumer : IConsumer<PublishErpRejectReplacementCostMessage>
    {
        private readonly IMediator _mediator;

        public RejectReplacementCostErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<PublishErpRejectReplacementCostMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpRejectReplacementCostDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
