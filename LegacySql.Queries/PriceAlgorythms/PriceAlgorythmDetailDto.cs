using System;

namespace LegacySql.Queries.PriceAlgorythms
{
    public class PriceAlgorythmDetailDto
    {
        public int Id { get; set; }
        public int ProductTypeId { get; set; }
        public Guid? ProductTypeGuid { get; set; }
        public string Vendor { get; set; }
        public Guid? VendorGuid { get; set; }
        public string Class { get; set; }
    }
}