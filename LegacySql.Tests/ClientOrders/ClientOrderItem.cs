namespace LegacySql.Tests.ClientOrders
{
    public class ClientOrderItem
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? ProductTypeId { get; set; }
        public int? Quantity { get; set; }
        public int ClientOrderId { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceUAH { get; set; }
        public decimal? PriceSS { get; set; }
        public short? Warranty { get; set; }
        public string Brand { get; set; }
        public int? ColCB { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal CrossRate { get; set; }
        public int? LevelBonusId { get; set; }
        public decimal? PriceRozn { get; set; }
    }
}
