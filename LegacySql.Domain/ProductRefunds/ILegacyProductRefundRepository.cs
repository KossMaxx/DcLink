using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ProductRefunds
{
    public interface ILegacyProductRefundRepository
    {
        IAsyncEnumerable<ProductRefund> GetChangedAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<ProductRefund> GetProductRefundAsync(int id, CancellationToken cancellationToken);
    }
}
