using System.Collections.Generic;
using MediatR;
using MessageBus.BankPayments.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpBankPaymentEvent : INotification
    {
        public ResaveErpBankPaymentEvent(List<ErpBankPaymentDto> messages)
        {
            Messages = messages;
        }

        public List<ErpBankPaymentDto> Messages { get; }
    }
}
