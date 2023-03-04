using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.FreeDocuments.Import;
using MessageBus.FreeDocuments.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class FreeDocumentErpSaveConsumer : BaseConsumer<PublishErpFreeDocumentMessage>
    {
        private readonly IMediator _mediator;

        public FreeDocumentErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<PublishErpFreeDocumentMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpFreeDocumentDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
