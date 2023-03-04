using System.Collections.Generic;
using System.Threading;

namespace LegacySql.Domain.Rates
{
    public interface ILegacyRateRepository
    {
        IAsyncEnumerable<Rate> GetRatesAsync(CancellationToken cancellationToken);
    }
}
