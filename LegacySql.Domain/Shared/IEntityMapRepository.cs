using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LegacySql.Domain.Shared
{
    public interface IEntityMapRepository
    {
        Task<ExternalMap> GetByMapAsync(Guid mapGuid);
        Task<ExternalMap> GetByLegacyAsync(int legacyId);
        Task<ExternalMap> GetByErpAsync(Guid erpId);
        Task SaveAsync(ExternalMap newEntityMap, Guid? id = null);
        Task<IEnumerable<int>> GetAllLegacyIds();
        Task<bool> IsMappingExist(Guid id);
        Task DeleteByIdAsync(Guid id);
        Task<IEnumerable<ExternalMap>> GetAllFullMappingsAsync();
        Task<IEnumerable<ExternalMap>> GetRangeByErpIdsAsync(IEnumerable<Guid> data);
        Task<IEnumerable<ExternalMap>> GetAllAsync();
    }
}
