using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("kassa_list")]
    public class CashboxEF
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
