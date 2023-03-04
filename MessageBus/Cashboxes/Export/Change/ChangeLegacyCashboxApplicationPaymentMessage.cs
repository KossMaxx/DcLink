using System;

namespace MessageBus.Cashboxes.Export.Change
{
    public class ChangeLegacyCashboxApplicationPaymentMessage : BaseSagaMessage<CashboxApplicationPaymentDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
