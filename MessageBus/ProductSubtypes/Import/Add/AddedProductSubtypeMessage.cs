using System;

namespace MessageBus.ProductSubtypes.Import.Add
{
    public class AddedProductSubtypeMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
