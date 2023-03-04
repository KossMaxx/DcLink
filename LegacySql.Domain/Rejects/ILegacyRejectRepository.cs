using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Rejects
{
    public interface ILegacyRejectRepository
    {
        IAsyncEnumerable<Reject> GetChangedRejectsAsync(DateTime? lastChangedDate, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);    
        IAsyncEnumerable<Reject> GetOpenRejectsAsync(CancellationToken cancellationToken);    
        Task<Reject> GetRejectAsync(int id, CancellationToken cancellationToken);
    }
}
