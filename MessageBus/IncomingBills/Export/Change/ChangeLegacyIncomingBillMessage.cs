using System;

namespace MessageBus.IncomingBills.Export.Change
{
    public class ChangeLegacyIncomingBillMessage : BaseSagaMessage<IncomingBillDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
