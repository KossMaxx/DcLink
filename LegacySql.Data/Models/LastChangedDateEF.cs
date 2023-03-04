using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Data.Models
{
    public class LastChangedDateEF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string EntityType { get; set; }
        public DateTime? Date { get; set; }
    }
}
