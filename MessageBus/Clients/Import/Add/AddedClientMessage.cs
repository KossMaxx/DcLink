using System;

namespace MessageBus.Clients.Import.Add
{
    public class AddedClientMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
