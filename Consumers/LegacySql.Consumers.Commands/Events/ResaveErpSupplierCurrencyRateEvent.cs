using System.Collections.Generic;
using MediatR;
using MessageBus.SupplierCurrencyRates.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpSupplierCurrencyRateEvent : INotification
    {
        public ResaveErpSupplierCurrencyRateEvent(List<ErpSupplierCurrencyRateDto> messages)
        {
            Messages = messages;
        }

        public List<ErpSupplierCurrencyRateDto> Messages { get; }
    }
}
