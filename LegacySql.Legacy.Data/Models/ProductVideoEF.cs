using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("VideoURL")]
    public class ProductVideoEF
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual ProductEF Product { get; set; }
        public string Url { get; set; }
        public DateTime Date { get; set; }
    }
}
