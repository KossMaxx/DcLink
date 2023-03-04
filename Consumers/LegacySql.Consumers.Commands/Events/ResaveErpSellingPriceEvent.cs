using System.Collections.Generic;
using MediatR;
using MessageBus.SellingPrices.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpSellingPriceEvent : INotification
    {
        public ResaveErpSellingPriceEvent(List<ErpSellingPriceDto> messages)
        {
            Messages = messages;
        }

        public List<ErpSellingPriceDto> Messages { get; }
    }
}
