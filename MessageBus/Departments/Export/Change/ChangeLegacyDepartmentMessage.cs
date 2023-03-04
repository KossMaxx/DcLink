using System;

namespace MessageBus.Departments.Export.Change
{
    public class ChangeLegacyDepartmentMessage : BaseSagaMessage<DepartmentDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
