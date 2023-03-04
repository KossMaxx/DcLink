using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.PartnerProductGroups.Import;
using MessageBus.PartnerProductGroups.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class PartnerProductGroupErpSaveConsumer : BaseConsumer<PublishErpPartnerProductGroupMessage>
    {
        private readonly IMediator _mediator;

        public PartnerProductGroupErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpPartnerProductGroupMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpPartnerProductGroupDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
