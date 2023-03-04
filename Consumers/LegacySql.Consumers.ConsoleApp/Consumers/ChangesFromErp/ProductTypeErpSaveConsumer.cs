using System;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ProductTypes.Import;
using MessageBus.ProductTypes.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ProductTypeErpSaveConsumer : BaseConsumer<PublishErpProductTypeMessage>
    {
        private readonly IMediator _mediator;

        public ProductTypeErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpProductTypeMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ProductTypeErpDto>(message.Value, context.MessageId ?? Guid.NewGuid());
            await _mediator.Send(command);
        }
    }
}
