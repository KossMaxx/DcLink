using LegacySql.Domain.Shared;

namespace LegacySql.Domain.MarketSegments
{
    public class MarketSegment : Mapped
    {
        public MarketSegment(IdMap id, string title, bool hasMap) : base(hasMap)
        {
            Id = id;
            Title = title;
        }

        public IdMap Id { get; }
        public string Title { get; }

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
