using System;

namespace MessageBus.IncomingBills.Export
{
    public class IncomingBillItemDto
    {
        public decimal PriceUAH { get; set; }
        public int Quantity { get; set; }
        public Guid NomenclatureId { get; set; }
    }
}
