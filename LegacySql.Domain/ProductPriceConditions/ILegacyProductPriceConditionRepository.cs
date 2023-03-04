using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ProductPriceConditions
{
    public interface ILegacyProductPriceConditionRepository
    {
        Task<ProductPriceCondition> GetAsync(int id, CancellationToken cancellationToken);
        IAsyncEnumerable<ProductPriceCondition> GetAllAsync(CancellationToken cancellationToken);
    }
}
