using System;
using LegacySql.Domain.Shared;

namespace MessageBus.SellingPrices.Import
{
    public class ErpSellingPriceDto
    {
        public Guid ProductId { get; set; }
        public int ColumnId { get; set; }
        public decimal? Price { get; set; }
        public PaymentTypes PaymentType { get; set; }
    }
}
