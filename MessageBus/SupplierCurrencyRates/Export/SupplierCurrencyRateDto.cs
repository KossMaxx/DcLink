using System;

namespace MessageBus.SupplierCurrencyRates.Export
{
    public class SupplierCurrencyRateDto
    {
        public decimal? RateNal { get; set; }
        public decimal? RateBn { get; set; }
        public decimal? RateDdr { get; set; }
        public DateTime? Date { get; set; }
        public Guid? ClientId { get; set; }
        public bool ChangedByBot { get; set; }
        public byte? BalanceCurrency { get; set; }
        public bool? IsSupplier { get; set; }
        public bool? IsCustomer { get; set; }
    }
}
