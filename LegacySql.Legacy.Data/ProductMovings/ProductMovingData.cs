using System;

namespace LegacySql.Legacy.Data.ProductMovings
{
    public class ProductMovingData
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? ForShipmentDate { get; set; }
        public string CreatorUsername { get; set; }
        public int? CreatorId { get; set; }
        public int OutWarehouseId { get; set; }
        public int InWarehouseId { get; set; }
        public string Description { get; set; }
        public string Okpo { get; set; }
        public DateTime ChangedAt { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsForShipment { get; set; }
        public bool IsShipped { get; set; }
        public ProductMovingItemData Item { get; set; }
    }
}
