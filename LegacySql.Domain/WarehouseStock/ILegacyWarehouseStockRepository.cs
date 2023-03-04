using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.WarehouseStock
{
    public interface ILegacyWarehouseStockRepository
    {
        IAsyncEnumerable<ProductStock> GetChangedWarehouseStocksAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        IAsyncEnumerable<ProductStockReserved> GetChangedReservedWarehouseStocksAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        IAsyncEnumerable<ProductBNStock> GetChangedCompanyStocksAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        IAsyncEnumerable<ProductBNStockReserved> GetChangedReservedCompanyStocksAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
    }
}
