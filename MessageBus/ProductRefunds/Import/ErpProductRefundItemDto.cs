using System;

namespace MessageBus.ProductRefunds.Import
{
    public class ErpProductRefundItemDto
    {
        public Guid NomenclatureId { get; set; }
        public int Quantity { get; set; }
    }
}
