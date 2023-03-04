using System;

namespace MessageBus.ProductMovings.Import
{
    public class ErpProductMovingItemDto
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
    }
}
