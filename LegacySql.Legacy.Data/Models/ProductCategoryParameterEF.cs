using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("KategoryTovars")]
    public class ProductCategoryParameterEF
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int? ParameterId { get; set; }
    }
}
