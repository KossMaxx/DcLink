using System;

namespace MessageBus.Bills.Import.Add
{
    public class AddedBillMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
