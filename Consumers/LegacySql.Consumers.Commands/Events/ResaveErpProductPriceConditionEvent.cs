using System.Collections.Generic;
using MediatR;
using MessageBus.ProductPriceConditions.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpProductPriceConditionEvent : INotification
    {
        public ResaveErpProductPriceConditionEvent(List<ErpProductPriceConditionDto> messages)
        {
            Messages = messages;
        }

        public List<ErpProductPriceConditionDto> Messages { get; }
    }
}
