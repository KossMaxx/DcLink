using System.Collections.Generic;
using MediatR;
using MessageBus.Rejects.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpRejectReplacementCostEvent : INotification
    {
        public ResaveErpRejectReplacementCostEvent(List<ErpRejectReplacementCostDto> messages)
        {
            Messages = messages;
        }

        public List<ErpRejectReplacementCostDto> Messages { get; }
    }
}
