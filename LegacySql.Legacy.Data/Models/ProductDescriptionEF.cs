using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_description")]
    public class ProductDescriptionEF
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual ProductEF Product { get; set; }
        public int LanguageId { get; set; }
        public virtual LanguageEF Language { get; set; }
        public string Description { get; set; }
        public string Uuu { get; set; }
        public DateTime Date { get; set; }
    }
}
