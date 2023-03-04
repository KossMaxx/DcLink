using System;

namespace MessageBus.SupplierCurrencyRates.Import
{
    public class ErpSupplierCurrencyRateDto
    {
        public decimal? RateNal { get; set; }
        public decimal? RateBn { get; set; }
        public decimal? RateDdr { get; set; }
        public Guid ClientId { get; set; }
    }
}
