using System.Threading.Tasks;
using LegacySql.Consumers.Commands;
using MassTransit;
using MediatR;
using MessageBus.Bonuses.Import;
using MessageBus.Bonuses.Import.Add;

namespace LegacySql.Consumers.ConsoleApp.Consumers.ChangesFromErp
{
    public class BonusErpSaveConsumer : BaseConsumer<PublishErpBonusMessage>
    {
        private readonly IMediator _mediator;

        public BonusErpSaveConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task ConsumeMessage(ConsumeContext<PublishErpBonusMessage> context)
        {
            var message = context.Message;
            var command = new BaseSaveErpCommand<ErpBonusDto>(message.Value, message.MessageId);
            await _mediator.Send(command);
        }
    }
}
