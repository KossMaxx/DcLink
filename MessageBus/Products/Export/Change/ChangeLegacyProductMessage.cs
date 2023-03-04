using System;

namespace MessageBus.Products.Export.Change
{
    public class ChangeLegacyProductMessage : BaseSagaMessage<ProductDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
