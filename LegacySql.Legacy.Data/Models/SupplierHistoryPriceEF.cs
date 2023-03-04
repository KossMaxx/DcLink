using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("priceHistory")]
    public class SupplierHistoryPriceEF
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public decimal? Price { get; set; }
    }
}