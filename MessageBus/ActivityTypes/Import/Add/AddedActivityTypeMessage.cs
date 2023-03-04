using System;

namespace MessageBus.ActivityTypes.Import.Add
{
    public class AddedActivityTypeMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
