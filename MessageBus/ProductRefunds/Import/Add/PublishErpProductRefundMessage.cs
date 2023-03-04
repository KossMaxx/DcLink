namespace MessageBus.ProductRefunds.Import.Add
{
    public class PublishErpProductRefundMessage : BaseMessage
    {
        public ErpProductRefundDto Value { get; set; }
    }
}
