using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacySql.Domain.WarehouseStock
{
    public class ProductStockReserved
    {
        public ProductStockReserved(IdMap productId, IEnumerable<WarehouseStock> warehouseStocks,
            /* IEnumerable<CompanyStock> companyStocks,*/ IdMap supplierId, decimal cashPrice, decimal cashlessPrice)
        {
            ProductId = productId;
            WarehouseStocks = warehouseStocks;
            //CompanyStocks = companyStocks;
            SupplierId = supplierId;
            CashPrice = cashPrice;
            CashlessPrice = cashlessPrice;
        }

        public IdMap ProductId { get; }
        public IdMap SupplierId { get; }
        public IEnumerable<WarehouseStock> WarehouseStocks { get; }
        //public IEnumerable<CompanyStock> CompanyStocks { get; }
        public decimal CashPrice { get; }
        public decimal CashlessPrice { get; }

        public bool IsMappingsFull()
        {
            return ProductId?.ExternalId != null && WarehouseStocks.All(e => e.WarehouseId.ExternalId != null) && (SupplierId?.ExternalId != null || SupplierId == null);
        }
    }
}
