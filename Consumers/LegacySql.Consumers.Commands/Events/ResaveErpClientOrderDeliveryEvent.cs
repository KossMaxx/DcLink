using System.Collections.Generic;
using MediatR;
using MessageBus.ClientOrder.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpClientOrderDeliveryEvent : INotification
    {
        public ResaveErpClientOrderDeliveryEvent(List<ErpClientOrderDeliveryDto> messages)
        {
            Messages = messages;
        }

        public List<ErpClientOrderDeliveryDto> Messages { get; }
    }
}
