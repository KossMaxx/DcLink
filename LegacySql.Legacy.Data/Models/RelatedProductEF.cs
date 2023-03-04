using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("RelatedGoods")]
    public class RelatedProductEF
    {
        public int Id { get; set; }
        public ProductEF MainProduct { get; set; }
        public int MainProductId { get; set; }
        public ProductEF RelatedProduct { get; set; }
        public int RelatedProductId { get; set; }
        public DateTime? LogDate { get; set; }
    }
}
