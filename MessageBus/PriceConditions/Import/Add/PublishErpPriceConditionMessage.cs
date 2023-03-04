namespace MessageBus.PriceConditions.Import.Add
{
    public class PublishErpPriceConditionMessage : BaseMessage
    {
        public ErpPriceConditionDto Value { get; set; }
    }
}
