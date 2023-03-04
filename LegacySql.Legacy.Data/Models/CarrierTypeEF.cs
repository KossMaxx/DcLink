using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_carrier_types")]
    public class CarrierTypeEF
    {
        public short Id { get; set; }
        public string Title { get; set; }
    }
}
