using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.PartnerProductGroups
{
    public interface ILegacyPartnerProductGroupsRepository
    {
        IAsyncEnumerable<PartnerProductGroup> GetAllAsync(CancellationToken cancellationToken);
    }
}
