using System;
using System.Threading.Tasks;

namespace LegacySql.Domain.Warehouses
{
    public interface IWarehouseMapRepository
    {
        Task SaveAsync(WarehouseMap newMap, Guid? id = null);
        Task<WarehouseMap> GetByLegacyAsync(int legacyId);
        Task RemoveByErpAsync(Guid erpId);
        Task<WarehouseMap> GetByErpAsync(Guid erpId);
        Task<WarehouseMap> GetFirstMappingAsync();
    }
}
