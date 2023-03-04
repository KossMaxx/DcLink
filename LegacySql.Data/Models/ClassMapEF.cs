using System;

namespace LegacySql.Data.Models
{
    public class ClassMapEF
    {
        public Guid Id { get; set; }
        public Guid MapGuid { get; set; }
        public Guid? ErpGuid { get; set; }
        public string LegacyTitle { get; set; }
    }
}
