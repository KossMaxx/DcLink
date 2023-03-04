using System;

namespace MessageBus.IncomingBills.Import.Add
{
    public class AddedIncomingBillMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
