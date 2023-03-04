using System.Collections.Generic;
using MediatR;
using MessageBus.Bonuses.Import;
using MessageBus.Penalties.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpBonusEvent : INotification
    {
        public ResaveErpBonusEvent(List<ErpBonusDto> messages)
        {
            Messages = messages;
        }

        public List<ErpBonusDto> Messages { get; }
    }
}
