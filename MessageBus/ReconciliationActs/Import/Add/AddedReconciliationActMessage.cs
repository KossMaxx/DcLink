using System;

namespace MessageBus.ReconciliationActs.Import.Add
{
    public class AddedReconciliationActMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
