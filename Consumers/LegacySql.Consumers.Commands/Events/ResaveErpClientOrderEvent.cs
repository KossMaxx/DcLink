using System.Collections.Generic;
using MediatR;
using MessageBus.ClientOrder.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpClientOrderEvent : INotification
    {
        public ResaveErpClientOrderEvent(List<ErpClientOrderDto> messages)
        {
            Messages = messages;
        }

        public List<ErpClientOrderDto> Messages { get; }
    }
}
