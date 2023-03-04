using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Поступления")]
    public class IncomingBillItemEF
    {
        public int Id { get; set; }
        public int? IncomingBillId { get; set; } 
        public decimal? PriceUAH { get; set; }
        public int? Quantity { get; set; }
        public int? NomenclatureId { get; set; }
        public virtual ProductGeneralEF Nomenclature { get; set; }
    }
}
