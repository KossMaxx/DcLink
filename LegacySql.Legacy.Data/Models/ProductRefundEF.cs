using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("ПН")]
    public class ProductRefundEF
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        public byte Type { get; set; }
        public DateTime? ChangedAt { get; set; }
        public virtual ICollection<ProductRefundItemEF> Items { get; set; }
    }
}
