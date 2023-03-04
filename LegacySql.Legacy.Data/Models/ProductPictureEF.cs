using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("PicturesUrl")]
    public partial class ProductPictureEF
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual ProductEF Product { get; set; }
        public string Url { get; set; }
        public DateTime Date { get; set; }
        public string Uuu { get; set; }
        public byte CobraPic { get; set; }
    }
}
