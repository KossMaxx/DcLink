using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Products
{
    public interface ILegacyProductRepository
    {
        IAsyncEnumerable<Product> GetChangedProductsAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);    
        Task<Product> GetProductAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<int>> GetProductsIdsAsync(CancellationToken cancellationToken);
    }
}
