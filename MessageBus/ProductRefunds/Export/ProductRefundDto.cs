using System;
using System.Collections.Generic;

namespace MessageBus.ProductRefunds.Export
{
    public class ProductRefundDto
    {
        public Guid? Id { get; set; }
        public DateTime Date { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ClientOrderId { get; set; }
        public IEnumerable<ProductRefundItemDto> Items { get; set; }
    }
}
