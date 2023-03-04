using System;
using System.Collections.Generic;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.SellingPrices
{
    public class SellingPrice
    {
        public DateTime? Date { get; }
        public IdMap ProductId { get; }
        public int? ColumnId { get; }
        public decimal? Price { get; private set; }
        public string Algorithm { get; }
        public int? Currency { get; }
        public PaymentTypes PaymentType { get; }

        public SellingPrice(
            DateTime? date, 
            IdMap productId, 
            int? columnId, 
            decimal? price, 
            string algorithm, 
            int? currency, 
            PaymentTypes paymentType)
        {
            Date = date;
            ProductId = productId;
            ColumnId = columnId;
            Price = price;
            Algorithm = algorithm;
            Currency = currency;
            PaymentType = paymentType;
        }

        public bool IsMappingsFull()
        {
            return ProductId?.ExternalId != null;
        }

        public SellingPrice GetCopyWith(PaymentTypes paymentType)
        {
            return new SellingPrice(Date,ProductId,ColumnId,Price,Algorithm,Currency,paymentType);
        }

        public void ChangePrice(decimal? price)
        {
            Price = price;
        }
    }

    public class SellingPricePackage
    {
        public Guid? ProductId { get; set; }
        public List<SellingPrice> CurrencyList { get; set; }

        public SellingPricePackage()
        {
            CurrencyList = new List<SellingPrice>();
        }

        public bool IsMappingsFull()
        {
            return ProductId != null;
        }
    }
}
