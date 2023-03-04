using System;

namespace MessageBus.ProductPriceConditions.Export
{
    public class ProductPriceConditionDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProductId { get; set; }
        public short? Price { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal? Value { get; set; }
        public byte? Currency { get; set; }
    }
}
