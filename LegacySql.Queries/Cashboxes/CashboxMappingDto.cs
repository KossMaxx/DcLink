using System;

namespace LegacySql.Queries.Cashboxes
{
    public class CashboxMappingDto
    {
        public int SqlId { get; set; }
        public Guid ErpGuid { get; set; }
        public string Description { get; set; }
    }
}
