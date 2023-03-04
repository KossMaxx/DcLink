using System;

namespace MessageBus.Purchases.Import.Add
{
    public class AddedPurchaseMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
