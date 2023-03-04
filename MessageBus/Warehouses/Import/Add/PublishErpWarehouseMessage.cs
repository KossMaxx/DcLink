namespace MessageBus.Warehouses.Import.Add
{
    public class PublishErpWarehouseMessage : BaseMessage
    {
        public ErpWarehouseDto Value { get; set; }
    }
}
