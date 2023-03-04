namespace MessageBus.BankPayments.Import.Add
{
    public class PublishErpPaymentOrderMessage : BaseMessage
    {
        public ErpPaymentOrderDto Value { get; set; }
    }
}
