using System.Collections.Generic;

namespace MessageBus.ProductTypes.Export.Add
{
    public class AddProductTypeMessage : BaseSagaMessage<ProductTypeDto>
    {
        public IEnumerable<AddProductTypeCategoryMessage> Categories { get; set; }
    }
}
