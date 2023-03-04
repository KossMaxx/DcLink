using System;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductPriceConditions
{
    public class ProductPriceCondition
    {
        public IdMap Id { get; }
        public IdMap ClientId { get; }
        public IdMap ProductId { get; }
        public short? Price { get; }
        public DateTime? DateTo { get; }
        public decimal? Value { get; }
        public byte? Currency { get; }

        public ProductPriceCondition(IdMap id, IdMap clientId, IdMap productId, short? price, DateTime? dateTo, decimal? value, byte? currency)
        {
            Id = id;
            ClientId = clientId;
            ProductId = productId;
            Price = price;
            DateTo = dateTo;
            Value = value;
            Currency = currency;
        }

        public MappingInfo IsMappingsFull()
        {
            var why = new StringBuilder();
            
            if (ClientId?.ExternalId == null)
            {
                why.Append($"Поле: ClientId., Id: {ClientId?.InnerId}\n");
            }
            
            if (ProductId?.ExternalId == null)
            {
                why.Append($"Поле: ProductId., Id: {ProductId?.InnerId}\n");
            }

            var whyResult = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyResult),
                Why = whyResult,
            };
        }
    }
}
