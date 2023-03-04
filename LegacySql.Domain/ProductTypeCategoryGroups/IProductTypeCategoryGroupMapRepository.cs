using System;
using System.Threading.Tasks;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductTypeCategoryGroups
{
    public interface IProductTypeCategoryGroupMapRepository
    {
        Task SaveAsync(ProductTypeCategoryGroupMap newMap, Guid? id = null);
        Task<ProductTypeCategoryGroupMap> GetByMapAsync(Guid mapGuid);
        Task<ProductTypeCategoryGroupMap> GetByLegacyAsync(int legacyId);
        Task<Guid?> GetErpByLegacyAsync(int legacyId);
        Task<ProductTypeCategoryGroupMap> GetByErpAsync(Guid erpId);
        Task RemoveByErpAsync(Guid erpId);
    }
}
