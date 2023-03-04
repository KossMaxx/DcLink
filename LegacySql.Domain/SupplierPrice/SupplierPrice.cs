using System;
using System.Collections.Generic;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.SupplierPrice
{
    public class SupplierPrice
    {
        public int Id { get; }
        public DateTime? Date { get; }
        public IdMap ProductId { get; }
        public IdMap SupplierId { get; }
        public decimal? Price { get; }
        public decimal? PriceRetail { get; }
        public decimal? PriceDialer { get; }
        public string VendorCode { get; }
        public bool? Monitor { get; }
        public int? Currency { get; }
        public decimal? CurrencyRate { get; }
        public bool IsInStock { get; }
        public string Url { get; }
        public PaymentTypes PaymentType { get; }
        public string IsInStockText { get; }
        public DateTime? PriceDate { get; }

        public SupplierPrice(
            int id,
            DateTime? date, 
            IdMap productId, 
            IdMap supplierId, 
            decimal? price, 
            string supplierProductCode,
            string url,
            bool? monitor, 
            decimal? priceRetail, 
            decimal? priceDialer, 
            int? currency,
            bool isInStock,
            PaymentTypes paymentType,
            decimal? currencyRate, 
            string isInStockText, 
            DateTime? priceDate)
        {
            Id = id;
            Date = date;
            ProductId = productId;
            SupplierId = supplierId;
            Price = price;
            VendorCode = supplierProductCode;
            Monitor = monitor;
            PriceRetail = priceRetail;
            PriceDialer = priceDialer;
            Currency = currency;
            IsInStock = isInStock;
            CurrencyRate = currencyRate;
            Url = url;
            PaymentType = paymentType;
            IsInStockText = isInStockText;
            PriceDate = priceDate;
        }

        public MappingInfo IsMappingsFull()
        {
            var isMappingsFull = true;
            var why = new StringBuilder();
            if (ProductId != null && !ProductId.ExternalId.HasValue)
            {
                why.Append($"Поле: ProductId. Id: {ProductId?.InnerId}\n");
                isMappingsFull = false;
            }

            if (SupplierId != null && !SupplierId.ExternalId.HasValue)
            {
                why.Append($"Поле: SupplierId. Id: {SupplierId?.InnerId}\n");
                isMappingsFull = false;
            }

            return new MappingInfo
            {
                IsMappingFull = isMappingsFull,
                Why = why.ToString()
            };
        }
    }

    public class SupplierPricePackage
    {
        public Guid? Supplier { get; set; }
        public IEnumerable<SupplierPrice> SupplierPrices { get; set; }
    }
}
