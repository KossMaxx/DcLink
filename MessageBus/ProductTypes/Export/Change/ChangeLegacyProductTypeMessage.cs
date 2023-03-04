using System;
using System.Collections.Generic;

namespace MessageBus.ProductTypes.Export.Change
{
    public class ChangeLegacyProductTypeMessage : BaseSagaMessage<ProductTypeDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
        public IEnumerable<ChangeLegacyProductTypeCategoryMessage> Categories { get; set; }
    }
}
