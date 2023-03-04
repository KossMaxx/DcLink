using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Manufacturers.AddManufacturerMap;
using MassTransit;
using MediatR;
using MessageBus.Manufacturer.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class ManufacturerAddConsumer : BaseConsumer<AddedManufacturerMessage>
    {
        private readonly IMediator _mediator;
        public ManufacturerAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedManufacturerMessage> context)
        {
            var message = context.Message;
            var command = new AddManufacturerMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
