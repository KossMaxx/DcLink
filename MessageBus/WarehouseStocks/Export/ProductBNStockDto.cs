using System;
using System.Collections.Generic;

namespace MessageBus.WarehouseStocks.Export
{
    public class ProductBNStockDto
    {
        public Guid ProductId { get; set; }
        public IEnumerable<CompanyStockDto> CompanyStocks { get; set; }
        public Guid? SupplierId { get; set; }
        public decimal CashPrice { get; set; }
        public decimal CashlessPrice { get; set; }
    }
}
