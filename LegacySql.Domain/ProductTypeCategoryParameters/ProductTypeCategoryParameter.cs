using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductTypeCategoryParameters
{
    public class ProductTypeCategoryParameter : Mapped
    {
        public IdMap Id { get; private set; }
        public string Name { get; private set; }
        public string NameUA { get; private set; }

        public ProductTypeCategoryParameter(IdMap id, string name, string nameUA, bool hasMap) : base(hasMap)
        {
            Id = id;
            Name = name;
            NameUA = nameUA;
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
