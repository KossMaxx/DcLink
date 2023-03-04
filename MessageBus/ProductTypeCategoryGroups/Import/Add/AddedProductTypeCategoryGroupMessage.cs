using System;

namespace MessageBus.ProductTypeCategoryGroups.Import.Add
{
    public class AddedProductTypeCategoryGroupMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
