using System;

namespace LegacySql.Queries.PriceLists
{
    public class PriceListItemDto
    {
        public Guid? ProductId { get; set; }
        public decimal RetailPrice { get; set; }
        public string Computers { get; set; }
        public decimal Price { get; set; }
        public string Cash { get; set; }
        public string Inet { get; set; }
    }
}
