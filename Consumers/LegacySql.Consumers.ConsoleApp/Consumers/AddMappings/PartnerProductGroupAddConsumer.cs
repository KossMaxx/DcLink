using LegacySql.Consumers.Commands.PartnerProductGroups.AddPartnerProductGroup;
using MassTransit;
using MediatR;
using MessageBus.PartnerProductGroups.Import.Add;
using System.Threading.Tasks;

namespace LegacySql.Consumers.ConsoleApp.Consumers.AddMappings
{
    public class PartnerProductGroupAddConsumer : BaseConsumer<AddedPartnerProductGroupMessage>
    {
        private readonly IMediator _mediator;
        public PartnerProductGroupAddConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task ConsumeMessage(ConsumeContext<AddedPartnerProductGroupMessage> context)
        {
            var message = context.Message;
            var command = new AddPartnerProductGroupCommand(message.MessageId, message.Value);
            await _mediator.Send(command);
        }
    }
}
