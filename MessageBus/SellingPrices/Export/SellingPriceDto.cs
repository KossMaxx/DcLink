using System;
using System.Collections.Generic;
using LegacySql.Domain.Shared;

namespace MessageBus.SellingPrices.Export
{
    public class SellingPriceDto
    {
        public DateTime? Date { get; set; }
        public Guid ProductId { get; set; }
        public int? ColumnId { get; set; }
        public decimal? Price { get; set; }
        public string Algorithm { get; set; }
        public int? Currency { get; set; }
        public PaymentTypes PaymentType { get; set; }
    }

    public class SellingPricePackageDto
    {
        public Guid ProductId { get; set; }
        public List<SellingPriceDto> CurrencyList { get; set; }

        public SellingPricePackageDto()
        {
            CurrencyList = new List<SellingPriceDto>();
        }
    }
}
