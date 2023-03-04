using System;

namespace MessageBus.ProductTypes.Import.Add
{
    public class AddedProductTypeCategoryMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
