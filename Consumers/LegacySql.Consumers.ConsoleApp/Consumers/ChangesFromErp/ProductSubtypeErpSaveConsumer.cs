using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ProductSubtypes.Import;
using MessageBus.ProductSubtypes.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ProductSubtypeErpSaveConsumer : BaseConsumer<PublishErpProductSubtypeMessage>
    {
        private readonly IMediator _mediator;

        public ProductSubtypeErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpProductSubtypeMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpProductSubtypeDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
