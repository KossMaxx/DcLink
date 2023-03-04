using System;
using System.Collections.Generic;

namespace MessageBus.Purchases.Import
{
    public class ErpPurchaseDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public Guid? WarehouseId { get; set; }
        public Guid ClientId { get; set; }
        public int Quantity { get; set; }
        public IEnumerable<ErpPurchaseItemDto> Items { get; set; }
    }
}
