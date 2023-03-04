using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Firms
{
    public interface ILegacyFirmRepository
    {
        IAsyncEnumerable<Firm> GetChangedFirmsAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<Firm> GetFirmAsync(int id, CancellationToken cancellationToken);
    }
}
