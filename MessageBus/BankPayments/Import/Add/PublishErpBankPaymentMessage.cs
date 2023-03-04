namespace MessageBus.BankPayments.Import.Add
{
    public class PublishErpBankPaymentMessage : BaseMessage
    {
        public ErpBankPaymentDto Value { get; set; }
    }
}
