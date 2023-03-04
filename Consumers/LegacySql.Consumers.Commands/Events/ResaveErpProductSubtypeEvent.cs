using System.Collections.Generic;
using MediatR;
using MessageBus.ProductSubtypes.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpProductSubtypeEvent : INotification
    {
        public ResaveErpProductSubtypeEvent(List<ErpProductSubtypeDto> messages)
        {
            Messages = messages;
        }

        public List<ErpProductSubtypeDto> Messages { get; }
    }
}
