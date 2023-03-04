using System;
using LegacySql.Domain.ValueObjects;

namespace LegacySql.Domain.ProductDescriptions
{
    public class ProductDescription
    {
        public int Id { get; private set; }
        public ProductId ProductId { get; private set; }
        public LanguageId LanguageId { get; private set; }
        public string Description { get; private set; }
        public string Uuu { get; private set; }
        public DateTime Date { get; private set; }

        public ProductDescription(int id, ProductId productId, LanguageId languageId, string description, string uuu, DateTime date)
        {
            Id = id;
            ProductId = productId;
            LanguageId = languageId;
            Description = description;
            Uuu = uuu;
            Date = date;
        }
    }
}
