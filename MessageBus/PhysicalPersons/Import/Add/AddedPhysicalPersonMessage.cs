using System;

namespace MessageBus.PhysicalPersons.Import.Add
{
    public class AddedPhysicalPersonMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
