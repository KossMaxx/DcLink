using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacySql.Domain.Manufacturer
{
    public interface IManufacturerMapRepository
    {
        Task SaveAsync(ManufacturerMap newMap, Guid? id = null);
        Task<ManufacturerMap> GetByMapAsync(Guid mapGuid);
        Task<IEnumerable<int>> GetAllMapIdAsync();
        Task<ManufacturerMap> GetByErpAsync(Guid erpGuid);
    }
}
