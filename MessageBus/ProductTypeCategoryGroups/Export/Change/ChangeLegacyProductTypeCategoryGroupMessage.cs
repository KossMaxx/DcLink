using System;
using System.Collections.Generic;

namespace MessageBus.ProductTypeCategoryGroups.Export.Change
{
    public class ChangeLegacyProductTypeCategoryGroupMessage : BaseSagaMessage<ProductTypeCategoryGroupDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
