using System.Collections.Generic;
using System.Threading;

namespace LegacySql.Domain.ActivityTypes
{
    public interface ILegacyActivityTypesRepository
    {
        IAsyncEnumerable<ActivityType> GetAllAsync(CancellationToken cancellationToken);
    }
}
