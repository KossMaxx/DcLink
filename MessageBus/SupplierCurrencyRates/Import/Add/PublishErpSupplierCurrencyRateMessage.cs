namespace MessageBus.SupplierCurrencyRates.Import.Add
{
    public class PublishErpSupplierCurrencyRateMessage : BaseMessage
    {
        public ErpSupplierCurrencyRateDto Value { get; set; }
    }
}
