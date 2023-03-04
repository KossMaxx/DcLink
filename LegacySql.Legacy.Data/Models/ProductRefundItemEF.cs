using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Приход")]
    public class ProductRefundItemEF
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        
        public ProductRefundEF ProductRefund { get; set; }
        public int? ProductRefundId { get; set; }
        public int? ProductId { get; set; }
        public virtual ProductGeneralEF Product { get; set; }
    }
}
