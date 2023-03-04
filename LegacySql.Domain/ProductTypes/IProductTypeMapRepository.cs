using System;
using System.Threading.Tasks;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductTypes
{
    public interface IProductTypeMapRepository
    {
        Task SaveAsync(ProductTypeMap newMap, Guid? id = null);
        Task<ProductTypeMap> GetByMapAsync(Guid mapGuid);
        Task<ProductTypeMap> GetByLegacyAsync(int legacyId);
        Task<Guid?> GetErpByLegacyAsync(int legacyId);
        Task<ProductTypeMap> GetByErpAsync(Guid erpId);
        Task RemoveByErpAsync(Guid erpId);
    }
}
