namespace MessageBus.ClientOrder.Import.Add
{
    public class PublishErpClientOrderMessage : BaseMessage
    {
        public ErpClientOrderDto Value { get; set; }
    }
}
