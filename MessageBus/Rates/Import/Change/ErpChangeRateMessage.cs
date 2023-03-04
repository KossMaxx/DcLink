namespace MessageBus.Rates.Import.Change
{
    public class ErpChangeRateMessage : BaseMessage
    {
        public ErpRateDto Value { get; set; }
    }
}
