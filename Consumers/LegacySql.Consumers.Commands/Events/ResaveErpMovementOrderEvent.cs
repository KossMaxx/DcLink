using MediatR;
using MessageBus.MovementOrders.Import;
using System.Collections.Generic;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpMovementOrderEvent : INotification
    {
        public ResaveErpMovementOrderEvent(List<ErpMovementOrderDto> messages)
        {
            Messages = messages;
        }

        public List<ErpMovementOrderDto> Messages { get; }
    }
}
