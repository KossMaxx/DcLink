using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.NotFullMappings
{
    public class ErpNotFullMapped
    {
        public Guid Id { get; }
        public Guid ErpId { get; }
        public MappingTypes Type { get; }
        public DateTime Date { get; }
        public string Why { get; }
        public string Value { get; }

        public ErpNotFullMapped(Guid id, Guid erpId, MappingTypes type, DateTime date, string why, string value)
        {
            Id = id;
            ErpId = erpId;
            Type = type;
            Date = date;
            Why = why;
            Value = value;
        }

        public ErpNotFullMapped(Guid erpId, MappingTypes type, DateTime date, string why, string value)
        {
            ErpId = erpId;
            Type = type;
            Date = date;
            Why = why;
            Value = value;
        }
    }
}
