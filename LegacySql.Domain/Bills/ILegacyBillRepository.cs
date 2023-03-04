using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Bills
{
    public interface ILegacyBillRepository
    {
        IAsyncEnumerable<Bill> GetChangedBillOrdersAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<Bill> GetBillAsync(int id, CancellationToken cancellationToken);
    }
}
