using System;
using System.Collections.Generic;
using System.Text;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Classes
{
    public class ProductClass : Mapped
    {
        public IdMap Id { get; }
        public string Title { get; }
        public IEnumerable<IdMap> ProductTypes { get; }

        public ProductClass(IdMap id, string title, IEnumerable<IdMap> productTypes, bool hasMap) : base(hasMap)
        {
            Id = id;
            Title = title;
            ProductTypes = productTypes;
        }
        public MappingInfo IsMappingsFull()
        {
            var isMappingsFull = true;
            var why = new StringBuilder();
            foreach (var productType in ProductTypes)
            {
                if (!(productType is {ExternalId: null})) continue;
                why.Append($"Поле: ProductTypes. Id: {productType?.InnerId}\n");
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
