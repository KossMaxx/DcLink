using System.Collections.Generic;
using MediatR;
using MessageBus.Penalties.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpPenaltyEvent : INotification
    {
        public ResaveErpPenaltyEvent(List<ErpPenaltyDto> messages)
        {
            Messages = messages;
        }

        public List<ErpPenaltyDto> Messages { get; }
    }
}
