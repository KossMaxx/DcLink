using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Расход")]
    public class ClientOrderItemEF
    {
        public int Id { get; set; }
        public int? NomenclatureId { get; set; }
        public virtual ProductGeneralEF Nomenclature { get; set; }
        public int? Quantity { get; set; }
        public int ClientOrderId { get; set; }
        public DateTime? ChangedAt { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceUAH { get; set; }
        public short? Warranty { get; set; }
        public virtual ICollection<SoldProductSerialNumberEF> SerialNumbers { get; set; }
    }
}
