using System.Collections.Generic;
using MediatR;
using MessageBus.ClientOrder.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpClientOrderSerialNumbersEvent : INotification
    {
        public ResaveErpClientOrderSerialNumbersEvent(List<ErpClientOrderSerialNumbersDto> messages)
        {
            Messages = messages;
        }

        public List<ErpClientOrderSerialNumbersDto> Messages { get; }
    }
}
