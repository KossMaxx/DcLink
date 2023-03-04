using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.NotFullMappings
{
    public class NotFullMapped
    {
        public Guid Id { get; }
        public int InnerId { get; }
        public MappingTypes Type { get; }
        public DateTime Date { get; }
        public string Why { get; }

        public NotFullMapped(Guid id)
        {
            Id = id;
        }

        public NotFullMapped(Guid id, int innerId, MappingTypes type, DateTime date, string why) : this(id)
        {
            InnerId = innerId;
            Type = type;
            Date = date;
            Why = why;
        }

        public NotFullMapped(int innerId, MappingTypes type, DateTime date, string why)
        {
            InnerId = innerId;
            Type = type;
            Date = date;
            Why = why;
        }

        public NotFullMapped(int innerId, MappingTypes type)
        {
            InnerId = innerId;
            Type = type;
        }
    }
}
