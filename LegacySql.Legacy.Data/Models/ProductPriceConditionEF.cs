using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace LegacySql.Legacy.Data.Models
{
    [Table("HidenTovars")]
    public class ProductPriceConditionEF
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public ProductEF Product { get; set; }
        public short? Price { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal? Value { get; set; }
        public byte? Currency { get; set; }
    }
}
