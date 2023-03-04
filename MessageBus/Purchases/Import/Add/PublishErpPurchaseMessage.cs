namespace MessageBus.Purchases.Import.Add
{
    public class PublishErpPurchaseMessage : BaseMessage
    {
        public ErpPurchaseDto Value { get; set; }
    }
}
