using System.Collections.Generic;
using MediatR;
using MessageBus.Rejects.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpRejectEvent : INotification
    {
        public ResaveErpRejectEvent(List<ErpRejectDto> messages)
        {
            Messages = messages;
        }

        public List<ErpRejectDto> Messages { get; }
    }
}
