using System;

namespace LegacySql.Queries.PriceLists
{
    public class IndividualPriceListItemDto
    {
        public Guid? ProductId { get; set; }
        public string VendorCode { get; set; }
        public Guid? ProductTypeId { get; set; }
        public Guid? SubtypeId { get; set; }
        public Guid? ManufacturerId { get; set; }
        public string WorkName { get; set; }
        public decimal ClientPrice { get; set; }
        public int WarehouseQuantity { get; set; }
        public int Warranty { get; set; }
        public decimal? RetailPrice { get; set; }
        public bool IsMonitoring { get; set; }
    }
}
