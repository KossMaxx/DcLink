using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("ТоварыLogSS")]
    public class SellingPriceEF
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int? ProductId { get; set; }
        public short? ColumnId { get; set; }
        public decimal? Price { get; set; }
        public string Algorithm { get; set; }
        public int? Currency { get; set; }
        public DateTime? ProductDateLastPriceChange { get; set; }
        public bool IsCash { get; set; }
    }
}
