using System;

namespace MessageBus.ProductRefunds.Import.Add
{
    public class AddedProductRefundMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
