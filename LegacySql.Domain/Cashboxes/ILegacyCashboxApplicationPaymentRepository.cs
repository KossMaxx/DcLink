using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Cashboxes
{
    public interface ILegacyCashboxApplicationPaymentRepository
    {
        IAsyncEnumerable<CashboxApplicationPayment> GetChangedAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<CashboxApplicationPayment> Get(int id, CancellationToken cancellationToken);
        Task CarryingOutCashboxApplicationPayment(Guid id, Guid incomePaymentId, Guid outPaymentId, Guid userId, DateTime date, bool heldIn, CancellationToken cancellationToken);
    }
}
