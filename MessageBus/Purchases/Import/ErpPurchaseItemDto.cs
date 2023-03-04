using System;
using System.Collections.Generic;

namespace MessageBus.Purchases.Import
{
    public class ErpPurchaseItemDto
    {
        public Guid? ProductTypeId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductTitle { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public IEnumerable<string> SerialNumbers { get; set; }
    }
}
