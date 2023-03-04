using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Departments
{
    public class Department : Mapped
    {
        public Department(IdMap id, string title, string description, string bossPosition, IdMap bossId, bool hasMap) : base(hasMap)
        {
            Id = id;
            Title = title;
            Description = description;
            BossPosition = bossPosition;
            BossId = bossId;
        }

        public IdMap Id { get; }
        public string Title { get; }
        public string Description { get; }
        public string BossPosition { get;}
        public IdMap BossId { get; }

        public bool IsNew()
        {
            return !HasMap;
        }

        public bool IsChanged()
        {
            return Id?.ExternalId != null;
        }

        public MappingInfo IsMappingsFull()
        {
            var isMappingsFull = true;
            var why = new StringBuilder();

            if (!BossId.ExternalId.HasValue)
            {
                why.AppendLine($"Поле: BossId. Id: {BossId?.InnerId}");
                isMappingsFull = false;
            }

            return new MappingInfo
            {
                IsMappingFull = isMappingsFull,
                Why = why.ToString()
            };
        }
    }
}
