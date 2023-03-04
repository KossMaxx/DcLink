using System.Collections.Generic;
using MediatR;
using MessageBus.PriceConditions.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpPriceConditionEvent : INotification
    {
        public ResaveErpPriceConditionEvent(List<ErpPriceConditionDto> messages)
        {
            Messages = messages;
        }

        public List<ErpPriceConditionDto> Messages { get; }
    }
}
