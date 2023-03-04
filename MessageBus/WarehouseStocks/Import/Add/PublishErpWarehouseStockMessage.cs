namespace MessageBus.WarehouseStocks.Import.Add
{
    public class PublishErpWarehouseStockMessage : BaseMessage
    {
        public ErpWarehouseStockDto Value { get; set; }
    }
}
