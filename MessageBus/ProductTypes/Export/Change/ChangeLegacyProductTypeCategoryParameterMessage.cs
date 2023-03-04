using System;

namespace MessageBus.ProductTypes.Export.Change
{
    public class ChangeLegacyProductTypeCategoryParameterMessage : BaseSagaMessage<ProductTypeCategoryParameterDto>
    {
        public Guid? ErpId { get; set; }
    }
}
