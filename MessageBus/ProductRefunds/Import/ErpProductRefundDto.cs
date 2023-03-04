using System;
using System.Collections.Generic;

namespace MessageBus.ProductRefunds.Import
{
    public class ErpProductRefundDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid ClientId { get; set; }
        public Guid ClientOrderId { get; set; }
        public IEnumerable<ErpProductRefundItemDto> Items { get; set; }
    }
}
