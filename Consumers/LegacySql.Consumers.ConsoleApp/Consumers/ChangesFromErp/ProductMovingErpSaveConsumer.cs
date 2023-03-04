using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ProductMovings.Import;
using MessageBus.ProductMovings.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ProductMovingErpSaveConsumer : BaseConsumer<PublishErpProductMovingMessage>
    {
        private readonly IMediator _mediator;

        public ProductMovingErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<PublishErpProductMovingMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpProductMovingDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
