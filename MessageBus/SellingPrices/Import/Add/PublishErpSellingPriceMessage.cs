namespace MessageBus.SellingPrices.Import.Add
{
    public class PublishErpSellingPriceMessage : BaseMessage
    {
        public ErpSellingPriceDto Value { get; set; }
    }
}
