using MessageBus.ClientOrder.Import;

namespace MessageBus.ReconciliationActs.Import
{
    public class PublishErpReconciliationActMessage : BaseMessage
    {
        public ErpReconciliationActDto Value { get; set; }
    }
}
