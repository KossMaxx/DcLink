using System;

namespace MessageBus.WarehouseStocks.Export
{
    public class WarehouseStockDto
    {
        public Guid WarehouseId { get; set; }
        public int Quantity { get; set; } 
    }
}
