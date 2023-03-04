using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Domain.NotFullMappings;

namespace LegacySql.Domain.Shared
{
    public interface IErpNotFullMappedRepository
    {
        Task SaveAsync(ErpNotFullMapped mapping);
        Task<IEnumerable<ErpNotFullMapped>> GetAllAsync();
        Task RemoveAsync(Guid erpId, MappingTypes type);
        Task<ErpNotFullMapped> GetByErpIdAsync(Guid id, MappingTypes type);
        Task<IEnumerable<ErpNotFullMapped>> GetAllByTypeAsync(MappingTypes type);
    }
}
