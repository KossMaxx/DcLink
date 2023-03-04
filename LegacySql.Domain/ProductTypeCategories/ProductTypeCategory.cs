using System.Collections.Generic;
using System.Linq;
using LegacySql.Domain.ProductTypeCategoryGroups;
using LegacySql.Domain.ProductTypeCategoryParameters;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductTypes
{
    public class ProductTypeCategory : Mapped
    {
        public IdMap Id { get; }
        public string Name { get; }
        public string NameUA { get; }
        public bool Web { get; }
        public bool Web2 { get; }
        public bool PriceTag { get; }
        public ProductTypeCategoryGroup Group { get; }
        public IEnumerable<ProductTypeCategoryParameter> Parameters { get; }

        public ProductTypeCategory(IdMap id, string name, string nameUA, bool web, bool web2, bool priceTag, ProductTypeCategoryGroup group, bool hasMap, IEnumerable<ProductTypeCategoryParameter> parameters) : base(hasMap)
        {
            Id = id;
            Name = name;
            NameUA = nameUA;
            web = Web;
            Web2 = web2;
            PriceTag = priceTag;
            Group = group;
            Parameters = parameters;
        }

        public bool IsMappingsFull()
        {
            return Parameters.All(p => p.Id?.ExternalId != null);
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
