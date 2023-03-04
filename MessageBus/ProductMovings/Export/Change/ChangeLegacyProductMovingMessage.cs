using System;

namespace MessageBus.ProductMovings.Export.Change
{
    public class ChangeLegacyProductMovingMessage : BaseSagaMessage<ProductMovingDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
