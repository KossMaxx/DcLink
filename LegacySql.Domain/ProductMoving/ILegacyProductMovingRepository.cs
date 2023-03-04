using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ProductMoving
{
    public interface ILegacyProductMovingRepository
    {
        IAsyncEnumerable<ProductMoving> GetChangedProductMovingsAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<ProductMoving> GetProductMovingAsync(int id, CancellationToken cancellationToken);
    }
}
