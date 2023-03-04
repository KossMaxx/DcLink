using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("KategoryParameters")]
    public class ProductTypeCategoryParameterEF
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameUA { get; set; }
        public int CategoryId { get; set; }
        public virtual ProductTypeCategoryEF Category { get; set; }
    }
}
