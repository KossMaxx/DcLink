using System;

namespace MessageBus.Purchases.Export.Change
{
    public class ChangeLegacyPurchaseMessage : BaseSagaMessage<PurchaseDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
