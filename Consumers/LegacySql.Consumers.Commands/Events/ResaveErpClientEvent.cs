using System.Collections.Generic;
using MediatR;
using MessageBus.Clients.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpClientEvent : INotification
    {
        public ResaveErpClientEvent(List<ErpClientDto> messages)
        {
            Messages = messages;
        }

        public List<ErpClientDto> Messages { get; }
    }
}
