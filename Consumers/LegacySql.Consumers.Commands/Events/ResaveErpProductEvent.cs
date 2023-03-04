using System.Collections.Generic;
using MediatR;
using MessageBus.Products.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpProductEvent : INotification
    {
        public ResaveErpProductEvent(List<ErpProductDto> messages)
        {
            Messages = messages;
        }

        public List<ErpProductDto> Messages { get; }
    }
}
