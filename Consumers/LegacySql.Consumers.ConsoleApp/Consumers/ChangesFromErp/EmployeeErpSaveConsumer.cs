using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Employees.Import;
using MessageBus.Employees.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class EmployeeErpSaveConsumer : BaseConsumer<PublishErpEmployeeMessage>
    {
        private readonly IMediator _mediator;

        public EmployeeErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpEmployeeMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpEmployeeDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
