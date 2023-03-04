using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Типы")]
    public class ProductTypeEF
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        [NotMapped]
        public bool IsGroupe { get; set; }
        public int? MainId { get; set; }
        public DateTime? LastChangeDate { get; set; }
        public bool Web { get; set; }
        public string TypeNameUkr { get; set; }
        public Byte Status { get; set; }
        public virtual IEnumerable<ProductTypeCategoryEF> Categories { get; set; }
    }
}
