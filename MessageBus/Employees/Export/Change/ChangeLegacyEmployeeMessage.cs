using System;

namespace MessageBus.Employees.Export.Change
{
    public class ChangeLegacyEmployeeMessage : BaseSagaMessage<EmployeeDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
