using System.Collections.Generic;
using System.Threading;

namespace LegacySql.Domain.SegmentationTurnovers
{
    public interface ILegacySegmentationTurnoversRepository
    {
        IAsyncEnumerable<SegmentationTurnover> GetAllAsync(CancellationToken cancellationToken);
    }
}
