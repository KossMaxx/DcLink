using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Models
{
    public class CompanyStocksReservedEF
    {
        public int ProductId { get; set; }
        public int? ProductCashlessId { get; set; }
        public int? ProductSumItemReservedQty { get; set; }
        public int? SupplierId { get; set; }
        public decimal? FirstCost { get; set; }
        public decimal? PriceMinBNuah { get; set; }
        public IDictionary<int, int> CompanyStocks { get; set; }
    }
}
