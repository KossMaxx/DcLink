using LegacySql.Domain.ValueObjects;
using System.Collections.Generic;

namespace LegacySql.Legacy.Data.Models
{
    public class ProductStocksEF
    {
        public int ProductId { get; set; }
        public int? ProductCashlessId { get; set; }
        public int? ProductSumItemQty { get; set; }
        public int? ProductSumOcItemQty { get; set; }
        public int VirtualWarehouseQuantity { get; set; }
        public int? SupplierId { get; set; }
        public decimal? FirstCost { get; set; }
        public decimal? PriceMinBNuah { get; set; }
        public IDictionary<int, int> WarehouseStocks { get; set; }
    }
}
