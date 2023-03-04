using System;

namespace MessageBus.WarehouseStocks.Import
{
    public class ErpWarehouseStockDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public int Quantity { get; set; }
    }
}
