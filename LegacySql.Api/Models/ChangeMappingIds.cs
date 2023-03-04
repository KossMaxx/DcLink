using System;

namespace LegacySql.Api.Models
{
    public class ChangeMappingIds
    {
        public Guid OldId { get; set; }
        public Guid NewId { get; set; }
    }
}
