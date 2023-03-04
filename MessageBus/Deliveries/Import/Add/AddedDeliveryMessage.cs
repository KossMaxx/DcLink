using System;

namespace MessageBus.Deliveries.Import.Add
{
    public class AddedDeliveryMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
