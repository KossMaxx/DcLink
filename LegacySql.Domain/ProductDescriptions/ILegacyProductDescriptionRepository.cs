using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ProductDescriptions
{
    public interface ILegacyProductDescriptionRepository
    {
        Task<IEnumerable<ProductDescription>> GetByProductAsync(int productId, CancellationToken cancellationToken);
    }
}
