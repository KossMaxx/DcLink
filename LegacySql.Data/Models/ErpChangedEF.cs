using System;

namespace LegacySql.Data.Models
{
    public class ErpChangedEF
    {
        public Guid Id { get; set; }
        public int LegacyId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
    }
}
