using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Waybills.Import;
using MessageBus.Waybills.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class WaybillErpSaveConsumer : BaseConsumer<PublishErpWaybillMessage>
    {
        private readonly IMediator _mediator;

        public WaybillErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async override Task ConsumeMessage(ConsumeContext<PublishErpWaybillMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpWaybillDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
