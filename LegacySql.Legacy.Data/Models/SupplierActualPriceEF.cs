using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Цены")]
    public class SupplierActualPriceEF
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int ProductId { get; set; }
        public virtual ProductGeneralEF Product { get; set; }
        public int? SupplierId { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceRetail { get; set; }
        public decimal? PriceDialer { get; set; }
        public string SupplierProductCode { get; set; }
        public bool? Monitor { get; set; }
        public string IsInStock { get; set; }
        public int? Currency { get; set; }
    }
}
