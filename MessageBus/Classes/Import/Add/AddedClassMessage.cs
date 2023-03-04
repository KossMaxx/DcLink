using System;

namespace MessageBus.Classes.Import.Add
{
    public class AddedClassMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
