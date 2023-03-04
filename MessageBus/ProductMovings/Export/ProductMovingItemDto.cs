using System;

namespace MessageBus.ProductMovings.Export
{
    public class ProductMovingItemDto
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }
}
