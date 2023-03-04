using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ProductTypes
{
    public interface ILegacyProductTypeRepository
    {
        Task<(IEnumerable<ProductType> productType, DateTime? lastDate)> GetChangedProductTypesAsync(DateTime? changedAt, CancellationToken cancellationToken);
        Task<ProductType> Get(int id, CancellationToken cancellationToken);
        List<int> GetNotUsedTypesIds();
    }
}
