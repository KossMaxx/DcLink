using System;

namespace MessageBus.ProductRefunds.Export
{
    public class ProductRefundItemDto
    {
        public Guid? NomenclatureId { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
