using System;

namespace LegacySql.Commands.PriceAlgorythm.SaveErpPriceAlgorythm
{
    public class ErpPriceAlgorythmDetailDto
    {
        public int Id { get; set; }
        public Guid ProductTypeGuid { get; set; }
        public Guid? VendorGuid { get; set; }
        public string Class { get; set; }
    }
}