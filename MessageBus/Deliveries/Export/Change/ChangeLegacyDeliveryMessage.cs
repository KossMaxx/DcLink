using System;

namespace MessageBus.Deliveries.Export.Change
{
    public class ChangeLegacyDeliveryMessage : BaseSagaMessage<DeliveryDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
