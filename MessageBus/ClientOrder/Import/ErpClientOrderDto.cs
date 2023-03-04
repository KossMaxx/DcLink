using System;
using System.Collections.Generic;

namespace MessageBus.ClientOrder.Import
{
    public class ErpClientOrderDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime Date { get; set; }
        public ClientOrderStatuses Status { get; set; }
        public ClientOrderStates State { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public IEnumerable<ErpClientOrderItemDto> Items { get; set; }
    }
}
