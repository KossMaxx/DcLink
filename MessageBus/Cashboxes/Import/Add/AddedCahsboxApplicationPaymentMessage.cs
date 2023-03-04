using System;

namespace MessageBus.Cashboxes.Import.Add
{
    public class AddedCahsboxApplicationPaymentMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
