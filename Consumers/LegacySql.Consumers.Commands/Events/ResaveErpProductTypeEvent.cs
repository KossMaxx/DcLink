using System.Collections.Generic;
using MediatR;
using MessageBus.ProductTypes.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpProductTypeEvent : INotification
    {
        public ResaveErpProductTypeEvent(List<ProductTypeErpDto> messages)
        {
            Messages = messages;
        }

        public List<ProductTypeErpDto> Messages { get; }
    }
}
