using System.Collections.Generic;

namespace MessageBus.ProductTypes.Export.Add
{
    public class AddProductTypeCategoryMessage : BaseSagaMessage<ProductTypeCategoryDto>
    {
        public IEnumerable<AddProductTypeCategoryParameterMessage> Parameters { get; set; }
    }
}
