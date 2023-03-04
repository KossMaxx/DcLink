using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.PriceConditions
{
    public interface ILegacyPriceConditionRepository
    {
        Task<PriceCondition> GetAsync(int id, CancellationToken cancellationToken);
        IAsyncEnumerable<PriceCondition> GetAllAsync(CancellationToken cancellationToken);
    }
}
