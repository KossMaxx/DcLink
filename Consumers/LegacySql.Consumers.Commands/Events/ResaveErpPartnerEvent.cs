using System.Collections.Generic;
using MediatR;
using MessageBus.Clients.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpPartnerEvent : INotification

    {
        public ResaveErpPartnerEvent(List<ErpPartnerDto> messages)
        {
            Messages = messages;
        }

        public List<ErpPartnerDto> Messages { get; }
    }
}
