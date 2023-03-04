namespace MessageBus.ClientOrder.Import.Add
{
    public class PublishErpClientOrderSerialNumbersMessage : BaseMessage
    {
        public ErpClientOrderSerialNumbersDto Value { get; set; }
    }
}
