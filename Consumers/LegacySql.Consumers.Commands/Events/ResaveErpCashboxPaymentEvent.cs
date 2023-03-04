using System.Collections.Generic;
using MediatR;
using MessageBus.Cashboxes.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpCashboxPaymentEvent : INotification
    {
        public ResaveErpCashboxPaymentEvent(List<ErpCashboxPaymentDto> messages)
        {
            Messages = messages;
        }

        public List<ErpCashboxPaymentDto> Messages { get; }
    }
}
