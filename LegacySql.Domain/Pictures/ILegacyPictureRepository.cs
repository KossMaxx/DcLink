using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Pictures
{
    public interface ILegacyPictureRepository
    {
        Task<IEnumerable<ProductPicture>> GetByProductAsync(int productId, CancellationToken cancellationToken);
    }
}
