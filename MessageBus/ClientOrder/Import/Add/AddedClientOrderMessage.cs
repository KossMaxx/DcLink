using System;

namespace MessageBus.ClientOrder.Import.Add
{
    public class AddedClientOrderMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
