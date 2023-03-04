using System;
using System.Collections.Generic;

namespace MessageBus.ProductTypes.Export.Change
{
    public class ChangeLegacyProductTypeCategoryMessage : BaseSagaMessage<ProductTypeCategoryDto>
    {
        public Guid? ErpId { get; set; }
        public IEnumerable<ChangeLegacyProductTypeCategoryParameterMessage> Parameters { get; set; }
    }
}
