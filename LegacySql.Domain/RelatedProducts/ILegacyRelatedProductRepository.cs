using System;
using System.Collections.Generic;
using System.Threading;

namespace LegacySql.Domain.RelatedProducts
{
    public interface ILegacyRelatedProductRepository
    {
        IAsyncEnumerable<RelatedProduct> GetChangedAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        IAsyncEnumerable<RelatedProduct> GetRelatedProductAsync(int id, CancellationToken cancellationToken);
    }
}
