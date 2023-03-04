using System;

namespace MessageBus.ClientOrder.Import
{
    public class ErpClientOrderItemDto
    {
        public Guid NomenclatureId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceUAH { get; set; }
        public short Warranty { get; set; }
        public Guid? WarehouseId { get; set; }
    }
}
