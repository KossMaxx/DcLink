using System.Threading.Tasks;
using LegacySql.Consumers.Commands.PhysicalPersons.AddPhysicalPersonMap;
using MassTransit;
using MediatR;
using MessageBus.PhysicalPersons.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class PhysicalPersonAddConsumer : BaseConsumer<AddedPhysicalPersonMessage>
    {
        private readonly IMediator _mediator;
        public PhysicalPersonAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedPhysicalPersonMessage> context)
        {
            var message = context.Message;
            var command = new AddPhysicalPersonMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
