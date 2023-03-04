using System;
using System.Collections.Generic;
using LegacySql.Domain.Shared;

namespace MessageBus.SupplierPrices.Export
{
    public class SupplierPriceDto
    {
        public DateTime? Date { get; set; }
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public decimal Price { get; set; }
        public decimal PriceRetail { get; set; }
        public decimal PriceDialer { get; set; }
        public string VendorCode { get; set; }
        public bool? Monitor { get; set; }
        public int? Currency { get; set; }
        public decimal? CurrencyRate { get; set; }
        public bool IsInStock { get; set; }
        public string Url { get; set; }
        public PaymentTypes PaymentType { get; set; }
        public string Nal { get; set; }
        public DateTime? PriceDate { get; set; }
    }

    public class SupplierPricePackageDto
    {
        public Guid Supplier { get; set; }
        public List<SupplierPriceDto> SupplierPrices { get; set; }

        public SupplierPricePackageDto()
        {
            SupplierPrices = new List<SupplierPriceDto>();
        }

    }
}
