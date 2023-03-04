using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("kurses")]
    public class SupplierCurrencyRateEF
    {
        public int Id { get; set; }
        public decimal? RateNal { get; set; }
        public decimal? RateBn { get; set; }
        public decimal? RateDdr { get; set; }
        public DateTime? Date { get; set; }
        public int? ClientId { get; set; }
        public virtual ClientEF Client { get; set; }
        public string Partner { get; set; }
    }
}
