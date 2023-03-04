using System;

namespace LegacySql.Domain.ErpChanged
{
    public class ErpChanged
    {
        public ErpChanged(Guid? id)
        {
            Id = id;
        }

        public ErpChanged(int legacyId, DateTime date, string type, Guid? id = null) : this(id)
        {
            LegacyId = legacyId;
            Date = date;
            Type = type;
        }

        public Guid? Id { get; }
        public int LegacyId { get; }
        public DateTime Date { get; }
        public string Type { get; }
    }
}
