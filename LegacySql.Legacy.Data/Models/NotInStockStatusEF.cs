using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("PriceNalStatus")]
    public class NotInStockStatusEF
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
