using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ActivityTypes
{
    public class ActivityType : Mapped
    {
        public IdMap Id { get; }
        public string Title { get; }

        public ActivityType(IdMap id, string title, bool hasMap) : base(hasMap)
        {
            Id = id;
            Title = title;
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
