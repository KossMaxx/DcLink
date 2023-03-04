using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductTypeCategoryGroups
{
    public class ProductTypeCategoryGroup : Mapped
    {
        public IdMap Id { get; private set; }
        public string Name { get; private set; }
        public string NameUA { get; private set; }
        public int Sort { get; private set; }

        public ProductTypeCategoryGroup(IdMap id, string name, string nameUA, int sort, bool hasMap) : base(hasMap)
        {
            Id = id;
            Name = name;
            NameUA = nameUA;
            Sort = sort;
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
