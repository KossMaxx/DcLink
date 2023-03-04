using System;

namespace LegacySql.Tests.ClientOrders
{
    public class ClientOrder
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int ClientId { get; set; }
        public string MarketplaceNumber { get; set; }
        public string Manager { get; set; }
        public int? WarehouseId { get; set; }
        public int? Quantity { get; set; }
        public double? Amount { get; set; }
        public double? Rate { get; set; }
        public byte? Kolonka { get; set; }
        public DateTime? ReserveDate { get; set; }
        public DateTime? LastTime { get; set; }
        public DateTime? BalanceDate { get; set; }
        public int? WebUserId { get; set; }
    }
}
