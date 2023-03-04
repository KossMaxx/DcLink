using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Cashboxes
{
    public interface ILegacyCashboxPaymentRepository
    {
        IAsyncEnumerable<CashboxPayment> GetChangedAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<CashboxPayment> Get(int id, CancellationToken cancellationToken);
    }
}
