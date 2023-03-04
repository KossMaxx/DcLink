using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Employees.AddEmployeeMap;
using MassTransit;
using MediatR;
using MessageBus.Employees.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class EmployeeAddConsumer : BaseConsumer<AddedEmployeeMessage>
    {
        private readonly IMediator _mediator;
        public EmployeeAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<AddedEmployeeMessage> context)
        {
            var message = context.Message;
            var command = new AddEmployeeMapCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
