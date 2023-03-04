using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Счет")]
    public class BillEF
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int? ClientId { get; set; }
        public virtual ClientEF Client { get; set; }
        public string Comments { get; set; }
        public DateTime ChangedAt { get; set; }
        public int? Seller { get; set; }
        public DateTime ValidToDate { get; set; }
        public int? FirmId { get; set; }
        public virtual FirmEF Firm { get; set; }
        public string Creator { get; set; }
        public int? ManagerId { get; set; }
        public int? Number { get; set; }
        public bool? Issued { get; set; }
        public bool? IsPaid { get; set; }
        public Single? Rate { get; set; }
    }
}
