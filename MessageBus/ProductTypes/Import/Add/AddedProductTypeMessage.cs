using System;

namespace MessageBus.ProductTypes.Import.Add
{
    public class AddedProductTypeMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
