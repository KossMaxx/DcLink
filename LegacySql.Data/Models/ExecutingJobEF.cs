using System;
using System.ComponentModel.DataAnnotations;
using LegacySql.Domain.Shared;

namespace LegacySql.Data.Models
{
    public class ExecutingJobEF
    {
        [Key]
        public string JobType { get; set; }
    }
}
