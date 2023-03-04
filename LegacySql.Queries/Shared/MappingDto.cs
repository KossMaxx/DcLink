using System;

namespace LegacySql.Queries.Shared
{
    public class MappingDto
    {
        public int SqlId { get; set; }
        public Guid ErpGuid { get; set; }
        public string Title { get; set; }
    }
}
