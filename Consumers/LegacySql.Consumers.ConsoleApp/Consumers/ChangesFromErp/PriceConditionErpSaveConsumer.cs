using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.PriceConditions.Import;
using MessageBus.PriceConditions.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class PriceConditionErpSaveConsumer : BaseConsumer<PublishErpPriceConditionMessage>
    {
        private readonly IMediator _mediator;

        public PriceConditionErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpPriceConditionMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpPriceConditionDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
