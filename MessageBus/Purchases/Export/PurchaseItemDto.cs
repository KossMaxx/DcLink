using System;

namespace MessageBus.Purchases.Export
{
    public class PurchaseItemDto
    {
        public Guid? NomenclatureId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
