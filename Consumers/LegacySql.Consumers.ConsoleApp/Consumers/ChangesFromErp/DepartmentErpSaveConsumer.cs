using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Departments.Import;
using MessageBus.Departments.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class DepartmentErpSaveConsumer : BaseConsumer<PublishErpDepartmentMessage>
    {
        private readonly IMediator _mediator;

        public DepartmentErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpDepartmentMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpDepartmentDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
