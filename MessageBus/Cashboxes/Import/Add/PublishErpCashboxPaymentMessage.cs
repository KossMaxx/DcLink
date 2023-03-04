namespace MessageBus.Cashboxes.Import.Add
{
    public class PublishErpCashboxPaymentMessage : BaseMessage
    {
        public ErpCashboxPaymentDto Value { get; set; }
    }
}
