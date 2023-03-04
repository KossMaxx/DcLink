using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("SkladFree")]
    public class WarehouseStockEF
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductGeneralEF Product { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
    }
}
