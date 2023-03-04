using System;

namespace MessageBus.ProductTypes.Import.Add
{
    public class AddedProductTypeCategoryParameterMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
