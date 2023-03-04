using System;

namespace MessageBus.Employees.Import.Add
{
    public class AddedEmployeeMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
