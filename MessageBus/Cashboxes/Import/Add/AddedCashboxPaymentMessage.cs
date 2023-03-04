using System;

namespace MessageBus.Cashboxes.Import.Add
{
    public class AddedCashboxPaymentMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
