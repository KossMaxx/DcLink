using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("DostavkaTip")]
    public class DeliveryTypeEF
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public short CarrierTypeId { get; set; }
        public virtual CarrierTypeEF CarrierType { get; set; }
    }
}
