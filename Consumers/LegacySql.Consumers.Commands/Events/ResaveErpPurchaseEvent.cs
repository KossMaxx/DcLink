using System.Collections.Generic;
using MediatR;
using MessageBus.Purchases.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpPurchaseEvent : INotification
    {
        public ResaveErpPurchaseEvent(List<ErpPurchaseDto> messages)
        {
            Messages = messages;
        }

        public List<ErpPurchaseDto> Messages { get; }
    }
}
