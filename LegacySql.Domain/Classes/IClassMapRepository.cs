using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacySql.Domain.Classes
{
    public interface IClassMapRepository
    {
        Task SaveAsync(ClassMap newMap, Guid? id = null);
        Task<ClassMap> GetByMapAsync(Guid mapGuid);
        Task<IEnumerable<string>> GetAllMapTitlesAsync();
        Task<ClassMap> GetByErpAsync(Guid erpGuid);
    }
}
