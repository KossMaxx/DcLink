namespace MessageBus.Manufacturer.Import.Add
{
    public class PublishErpManufacturerMessage : BaseMessage
    {
        public ErpManufacturerDto Value { get; set; }
    }
}
