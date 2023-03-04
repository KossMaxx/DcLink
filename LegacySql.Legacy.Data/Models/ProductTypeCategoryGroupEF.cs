using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_specifications_subcategory")]
    public class ProductTypeCategoryGroupEF
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameUA { get; set; }
        public int Sort { get; set; }
    }
}
