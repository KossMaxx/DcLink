using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("BalanceLog")]
    public class ReconciliationActEF
    {
        public int Id { get; set; }
        public decimal? Sum { get; set; }
        public DateTime? ChangedAt { get; set; }
        public int? ClientId { get; set; }
        public bool? IsApproved { get; set; }
    }
}
