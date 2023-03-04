using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ProductPriceConditions.Import;
using MessageBus.ProductPriceConditions.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ProductPriceConditionErpSaveConsumer : BaseConsumer<PublishErpProductPriceConditionMessage>
    {
        private readonly IMediator _mediator;

        public ProductPriceConditionErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpProductPriceConditionMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpProductPriceConditionDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
