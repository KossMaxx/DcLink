using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Purchases
{
    public interface ILegacyPurchaseRepository
    {
        IAsyncEnumerable<Purchase> GetChangedAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        IAsyncEnumerable<Purchase> GetOpenAsync(CancellationToken cancellationToken);
        IAsyncEnumerable<Purchase> GetPurchaseAsync(int id, CancellationToken cancellationToken);
    }
}
