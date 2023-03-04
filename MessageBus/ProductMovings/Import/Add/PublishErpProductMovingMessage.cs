namespace MessageBus.ProductMovings.Import.Add
{
    public class PublishErpProductMovingMessage : BaseMessage
    {
        public ErpProductMovingDto Value { get; set; }
    }
}
