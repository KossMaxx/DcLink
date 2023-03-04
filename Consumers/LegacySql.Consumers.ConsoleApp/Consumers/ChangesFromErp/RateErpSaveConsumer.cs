using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Rates.Import;
using MessageBus.Rates.Import.Change;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class RateErpSaveConsumer : BaseConsumer<ErpChangeRateMessage>
    {
        private readonly IMediator _mediator;

        public RateErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<ErpChangeRateMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpRateDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
