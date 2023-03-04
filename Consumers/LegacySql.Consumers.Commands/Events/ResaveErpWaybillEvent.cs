using MediatR;
using MessageBus.Waybills.Import;
using System.Collections.Generic;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpWaybillEvent : INotification
    {
        public ResaveErpWaybillEvent(List<ErpWaybillDto> messages)
        {
            Messages = messages;
        }

        public List<ErpWaybillDto> Messages { get; }
    }
}
