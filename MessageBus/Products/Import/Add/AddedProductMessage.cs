using System;

namespace MessageBus.Products.Import.Add
{
    public class AddedProductMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
