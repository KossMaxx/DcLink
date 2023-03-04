namespace MessageBus.ProductSubtypes.Import.Add
{
    public class PublishErpProductSubtypeMessage : BaseMessage
    {
        public ErpProductSubtypeDto Value { get; set; }
    }
}
