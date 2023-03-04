using System;

namespace MessageBus.Bills.Export.Change
{
    public class ChangeLegacyBillMessage : BaseSagaMessage<BillDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
