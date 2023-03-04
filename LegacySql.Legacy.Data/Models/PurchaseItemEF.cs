using System;
using System.ComponentModel.DataAnnotations.Schema;
using LegacySql.Domain.Purchases;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Приход")]
    public class PurchaseItemEF
    {
        public int Id { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        
        public PurchaseEF Purchase { get; set; }
        public int? PurchaseId { get; set; }
        public ProductGeneralEF Product { get; set; }
        public int? ProductId { get; set; }
    }
}
