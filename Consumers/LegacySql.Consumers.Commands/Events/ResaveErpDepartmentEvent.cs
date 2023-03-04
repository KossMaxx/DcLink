using System.Collections.Generic;
using MediatR;
using MessageBus.Departments.Import;

namespace LegacySql.Consumers.Commands.Events
{
    public class ResaveErpDepartmentEvent : INotification
    {
        public ResaveErpDepartmentEvent(List<ErpDepartmentDto> messages)
        {
            Messages = messages;
        }

        public List<ErpDepartmentDto> Messages { get; }
    }
}
