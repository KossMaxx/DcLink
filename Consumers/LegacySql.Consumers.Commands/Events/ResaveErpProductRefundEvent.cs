using System.Collections.Generic;
using MediatR;
using MessageBus.ProductRefunds.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpProductRefundEvent : INotification
    {
        public ResaveErpProductRefundEvent(List<ErpProductRefundDto> messages)
        {
            Messages = messages;
        }

        public List<ErpProductRefundDto> Messages { get; }
    }
}
