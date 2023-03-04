using System;
using System.Collections.Generic;

namespace MessageBus.ClientOrder.Export
{
    public class ClientOrderItemDto
    {
        public Guid? NomenclatureId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceUAH { get; set; }
        public short Warranty { get; set; }
        public IEnumerable<string> SerialNumbers { get; set; }
    }
}
