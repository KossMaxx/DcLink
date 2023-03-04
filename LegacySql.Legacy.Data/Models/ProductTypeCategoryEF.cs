using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("KategorysByTip")]
    public class ProductTypeCategoryEF
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameUA { get; set; }
        public int TypeId { get; set; }
        public bool Web { get; set; }
        public bool Web2 { get; set; }
        public bool PriceTag { get; set; }
        public int? GroupId { get; set; }
        public virtual ProductTypeCategoryGroupEF Group { get; set; }
        public virtual ProductTypeEF Type { get; set; }
        public virtual IEnumerable<ProductTypeCategoryParameterEF> Parameters { get; set; }
    }
}
