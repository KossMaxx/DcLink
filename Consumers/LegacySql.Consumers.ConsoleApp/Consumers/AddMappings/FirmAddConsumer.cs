using LegacySql.Consumers.Commands.Firms.AddFirmMap;
using MassTransit;
using MediatR;
using MessageBus.Firms.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class FirmAddConsumer : BaseConsumer<AddedFirmMessage>
    {
        private readonly IMediator _mediator;
        public FirmAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async override Task ConsumeMessage(ConsumeContext<AddedFirmMessage> context)
        {
            var message = context.Message;
            var command = new AddFirmMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
