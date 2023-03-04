using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Rates")]
    public class RateEF
    {
        public byte Id { get; set; }
        public string Title { get; set; }
        public decimal Value { get; set; }
    }
}
