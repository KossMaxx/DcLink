namespace MessageBus.Products.Import.Add
{
    public class PublishErpProductMessage : BaseMessage
    {
        public ErpProductDto Value { get; set; }
    }
}
