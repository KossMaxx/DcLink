using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;

namespace LegacySql.Domain.Shared
{
    public interface INotFullMappedRepository
    {
        Task<IEnumerable<int>> GetIdsAsync(MappingTypes type);
        Task RemoveAsync(NotFullMapped mapping);
        Task SaveAsync(NotFullMapped mapping);
    }
}
