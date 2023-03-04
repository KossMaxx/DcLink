using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.SellingPrices
{
    public interface ILegacySellingPriceRepository
    {
        IAsyncEnumerable<SellingPricePackage> GetChangedSellingPricesAsync(DateTime? changedAt, CancellationToken cancellationToken);
        IAsyncEnumerable<SellingPricePackage>GetInitialSellingPricesAsync(int? productId, CancellationToken cancellationToken);
    }
}
