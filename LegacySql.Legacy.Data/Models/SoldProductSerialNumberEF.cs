using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("rushod")]
    public class SoldProductSerialNumberEF
    {
        public int Id { get; set; }
        public int ClientOrderItemId { get; set; }
        public virtual ClientOrderItemEF ClientOrderItem { get; set; }
        public string SerialNumber { get; set; }
        public int ClientOrderItemArchivalId { get; set; }
        public virtual ClientOrderItemArchivalEF ClientOrderItemArchival { get; set; }
    }
}
