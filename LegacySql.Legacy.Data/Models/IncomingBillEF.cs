using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("СчетПН")]
    public class IncomingBillEF
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string IncomingNumber { get; set; }
        public int RecipientId { get; set; }
        public int? SupplierId { get; set; }
        public virtual FirmEF Supplier { get; set; }
        public int? ClientId { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
}
