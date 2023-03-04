using System;

namespace MessageBus.ProductPriceConditions.Import
{
    public class ErpProductPriceConditionDto
    {
        public ImportStatus Status { get; set; }
        
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProductId { get; set; }
        public short? Price { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal? Value { get; set; }
    }
}
