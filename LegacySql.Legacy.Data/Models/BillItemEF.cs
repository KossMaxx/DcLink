using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("выписка")]
    public class BillItemEF
    {
        public int Id { get; set; }
        public int? BillId { get; set; }
        public int NomenclatureId { get; set; }
        public virtual ProductEF Nomenclature { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceUAH { get; set; }
        public int? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountUAH { get; set; }
    }
}
