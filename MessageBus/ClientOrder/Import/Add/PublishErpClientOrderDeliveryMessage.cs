namespace MessageBus.ClientOrder.Import.Add
{
    public class PublishErpClientOrderDeliveryMessage : BaseMessage
    {
        public ErpClientOrderDeliveryDto Value { get; set; }
    }
}
