using System;

namespace MessageBus.Firms.Import.Add
{
    public class AddedFirmMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
