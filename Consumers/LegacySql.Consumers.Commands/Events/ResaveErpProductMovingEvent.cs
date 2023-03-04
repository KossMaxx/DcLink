using MediatR;
using MessageBus.ProductMovings.Import;
using System.Collections.Generic;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpProductMovingEvent : INotification
    {
        public ResaveErpProductMovingEvent(List<ErpProductMovingDto> messages)
        {
            Messages = messages;
        }

        public List<ErpProductMovingDto> Messages { get; }
    }
}
