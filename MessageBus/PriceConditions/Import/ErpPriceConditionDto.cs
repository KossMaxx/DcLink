using System;

namespace MessageBus.PriceConditions.Import
{
    public class ErpPriceConditionDto
    {
        public ImportStatus Status { get; set; }
        
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ProductTypeId { get; set; }
        public Guid? Vendor { get; set; }
        public string ProductManager { get; set; }
        public Guid? ProductManagerId { get; set; }
        public short? PriceType { get; set; }
        public DateTime? DateTo { get; set; }
        public string Comment { get; set; }
        public decimal? Value { get; set; }
        public decimal? PercentValue { get; set; }
        public int? UpperThresholdPriceType { get; set; }
    }
}
