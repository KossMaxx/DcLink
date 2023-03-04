using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("podtip")]
    public class ProductSubtypeEF
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ProductTypeId { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
