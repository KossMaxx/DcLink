using System;

namespace MessageBus.ProductRefunds.Export.Change
{
    public class ChangeLegacyProductRefundMessage : BaseSagaMessage<ProductRefundDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
