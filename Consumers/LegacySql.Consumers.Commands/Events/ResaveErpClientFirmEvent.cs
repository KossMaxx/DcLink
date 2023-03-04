using System.Collections.Generic;
using MediatR;
using MessageBus.Clients.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpClientFirmEvent : INotification
    {
        public ResaveErpClientFirmEvent(List<ErpFirmDto> messages)
        {
            Messages = messages;
        }

        public List<ErpFirmDto> Messages { get; }
    }
}
