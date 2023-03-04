using System;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.ProductTypeCategoryGroups.Import;
using MessageBus.ProductTypeCategoryGroups.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ProductTypeCategoryGroupErpSaveConsumer : BaseConsumer<PublishErpProductTypeCategoryGroupMessage>
    {
        private readonly IMediator _mediator;

        public ProductTypeCategoryGroupErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpProductTypeCategoryGroupMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ProductTypeCategoryGroupErpDto>(message.Value, context.MessageId ?? Guid.NewGuid());
            await _mediator.Send(command);
        }
    }
}
