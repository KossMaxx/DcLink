using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ProductSubtypes
{
    public interface ILegacyProductSubtypeRepository
    {
        IAsyncEnumerable<ProductSubtype> GetChangedProductSubtypesAsync(DateTime? changedAt, CancellationToken cancellationToken);
        Task<ProductSubtype> GetAsync(int id, CancellationToken cancellationToken);
    }
}
