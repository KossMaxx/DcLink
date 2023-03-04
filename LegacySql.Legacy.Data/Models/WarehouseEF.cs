using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Склады")]
    public class WarehouseEF
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsСommission { get; set; }
    }
}
