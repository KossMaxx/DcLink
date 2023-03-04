using System;

namespace MessageBus.Cashboxes.Export.Change
{
    public class ChangeLegacyCashboxPaymentMessage : BaseSagaMessage<CashboxPaymentDto>, IMappedMessage
    {
        public Guid ErpId { get ; set ; }
    }
}
