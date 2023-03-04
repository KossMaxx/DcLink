using System;

namespace MessageBus.Departments.Import.Add
{
    public class AddedDepartmentMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
