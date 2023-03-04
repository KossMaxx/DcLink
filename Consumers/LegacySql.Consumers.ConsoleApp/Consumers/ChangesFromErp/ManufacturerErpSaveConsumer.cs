using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Manufacturer.Import;
using MessageBus.Manufacturer.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class ManufacturerErpSaveConsumer : BaseConsumer<PublishErpManufacturerMessage>
    {
        private readonly IMediator _mediator;

        public ManufacturerErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpManufacturerMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpManufacturerDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
