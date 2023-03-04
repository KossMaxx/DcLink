namespace MessageBus.ProductPriceConditions.Import.Add
{
    public class PublishErpProductPriceConditionMessage : BaseMessage
    {
        public ErpProductPriceConditionDto Value { get; set; }
    }
}
