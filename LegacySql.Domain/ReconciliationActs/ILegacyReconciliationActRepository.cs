using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ReconciliationActs
{
    public interface ILegacyReconciliationActRepository
    {
        IAsyncEnumerable<ReconciliationAct> GetChangedReconciliationActsAsync(DateTime? lastChangedDate, List<int> notFullMappingIds, CancellationToken cancellationToken);  
        Task<ReconciliationAct> GetReconciliationActAsync(int id, CancellationToken cancellationToken);
    }
}
