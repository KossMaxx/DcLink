using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_MP_delivery")]
    public class MarketplaceDeliveryEF
    {
        public int Id { get; set; }
        public string WarehouseNumber { get; set; }
        public string WarehouseId { get; set; }
        public int MarketplaceNumber { get; set; }
    }
}