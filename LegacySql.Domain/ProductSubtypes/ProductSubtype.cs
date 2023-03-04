using System;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductSubtypes
{
    public class ProductSubtype : Mapped
    {
        public IdMap Id { get; }
        public string Title { get; }
        public IdMap ProductTypeId { get; }
        public DateTime ChangedAt { get; }

        public ProductSubtype(IdMap id, string title, IdMap productTypeId, bool hasMap, DateTime changedAt) : base(hasMap)
        {
            Id = id;
            Title = title;
            ProductTypeId = productTypeId;
            ChangedAt = changedAt;
        }

        public MappingInfo IsMappingsFull()
        {
            var isMappingsFull = true;
            var why = new StringBuilder();
            if (ProductTypeId != null && !ProductTypeId.ExternalId.HasValue)
            {
                why.Append($"Поле: ProductTypeId. Id: {ProductTypeId?.InnerId}\n");
                isMappingsFull = false;
            }

            return new MappingInfo
            {
                IsMappingFull = isMappingsFull,
                Why = why.ToString()
            };
        }

        public bool IsNew()
        {
            return !HasMap;
        }

        public bool IsChanged()
        {
            return Id?.ExternalId != null;
        }
    }
}
