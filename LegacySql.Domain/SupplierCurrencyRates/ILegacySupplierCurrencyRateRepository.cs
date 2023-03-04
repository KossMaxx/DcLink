using System;
using System.Collections.Generic;
using System.Threading;

namespace LegacySql.Domain.SupplierCurrencyRates
{
    public interface ILegacySupplierCurrencyRateRepository
    {
        IAsyncEnumerable<SupplierCurrencyRate> GetChangedSupplierCurrencyRatesAsync(DateTime? changedAt, CancellationToken cancellationToken);
        IAsyncEnumerable<SupplierCurrencyRate> GetSupplierCurrencyRateAsync(int id, CancellationToken cancellationToken);
    }
}
