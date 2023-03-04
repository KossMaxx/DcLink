using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.MarketSegments
{
    public interface ILegacyMarketSegmentRepository
    {
        Task<IEnumerable<MarketSegment>> GetAllAsync(CancellationToken cancellationToken);
    }
}
