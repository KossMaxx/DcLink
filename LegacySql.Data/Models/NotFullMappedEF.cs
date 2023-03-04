using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LegacySql.Domain.Shared;

namespace LegacySql.Data.Models
{
    public class NotFullMappedEF
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int InnerId { get; set; }
        public MappingTypes Type { get; set; }
        public DateTime Date { get; set; }
        public string Why { get; set; }
    }
}
