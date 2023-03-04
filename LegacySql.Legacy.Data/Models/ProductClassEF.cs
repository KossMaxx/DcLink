using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LegacySql.Legacy.Data.Models
{
    public class ProductClassEF
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<ProductTypeEF> ProductTypes { get; set; }
        }
    }
