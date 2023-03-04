using System;
using System.Threading.Tasks;

namespace LegacySql.Domain.ProductSubtypes
{
    public interface IProductSubtypeMapRepository
    {
        Task SaveAsync(ProductSubtypeMap newMap, Guid? id = null);
        Task<ProductSubtypeMap> GetByMapAsync(Guid mapGuid);
        Task<ProductSubtypeMap> GetByLegacyAsync(int legacyId);
        Task<ProductSubtypeMap> GetByErpAsync(Guid erpId);
        Task RemoveByErpAsync(Guid erpId);
    }
}
