namespace MessageBus.MarketSegments.Import.Add
{
    public class PublishErpMarketSegmentMessage : BaseMessage
    {
        public ErpMarketSegmentDto Value { get; set; }
    }
}
