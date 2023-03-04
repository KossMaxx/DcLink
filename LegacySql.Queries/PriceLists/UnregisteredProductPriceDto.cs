using System;

namespace LegacySql.Queries.PriceLists
{
    public class UnregisteredProductPriceDto
    {
        public DateTime ImportDate { get; set; }
        public string CounterPartyPrice { get; set; }
        public string PosCode { get; set; }
        public string GoodsId { get; set; }
        public string GoodsCategory { get; set; }
        public string Manufacturer { get; set; }
        public string VendorCode { get; set; }
        public string GoodsName { get; set; }
        public string Warranty { get; set; }
        public int? Weight { get; set; }
        public int? Volume { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? Depth { get; set; }
        public string EAN { get; set; }
        public string ZEDCode { get; set; }
        public string RegistarionCountry { get; set; }
        public string OriginCountry { get; set; }
        public string givenCostOfPrice { get; set; }
    }
}
