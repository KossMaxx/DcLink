using MediatR;
using MessageBus.BankPayments.Import;
using System.Collections.Generic;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpPaymentOrderEvent : INotification
    {
        public ResaveErpPaymentOrderEvent(List<ErpPaymentOrderDto> messages)
        {
            Messages = messages;
        }

        public List<ErpPaymentOrderDto> Messages { get; }
    }
}
