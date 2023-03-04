using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Departments.AddDepartmentMap;
using MassTransit;
using MediatR;
using MessageBus.Departments.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class DepartmentAddConsumer : BaseConsumer<AddedDepartmentMessage>
    {
        private readonly IMediator _mediator;

        public DepartmentAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedDepartmentMessage> context)
        {
            var message = context.Message;
            var command = new AddDepartmentMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
