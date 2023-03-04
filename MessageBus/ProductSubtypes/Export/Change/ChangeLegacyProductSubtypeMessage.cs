using System;

namespace MessageBus.ProductSubtypes.Export.Change
{
    public class ChangeLegacyProductSubtypeMessage : BaseSagaMessage<ProductSubtypeDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
