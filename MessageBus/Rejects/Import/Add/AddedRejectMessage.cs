using System;

namespace MessageBus.Rejects.Import.Add
{
    public class AddedRejectMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
