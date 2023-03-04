namespace MessageBus.ProductTypes.Import.Add
{
    public class PublishErpProductTypeMessage : BaseMessage
    {
        public ProductTypeErpDto Value { get; set; }
    }
}
