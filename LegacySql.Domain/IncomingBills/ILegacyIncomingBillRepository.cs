using System;
using System.Collections.Generic;
using System.Threading;

namespace LegacySql.Domain.IncomingBills
{
    public interface ILegacyIncomingBillRepository
    {
        IAsyncEnumerable<IncomingBill> GetChangedIncomingBillOrdersAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        IAsyncEnumerable<IncomingBill> GetIncomingBillAsync(int id, CancellationToken cancellationToken);
    }
}
