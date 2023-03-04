using System;

namespace MessageBus.Bills.Export
{
    public class BillItemDto
    {
        public Guid NomenclatureId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceUAH { get; set; }
        public short Warranty { get; set; }
    }
}
