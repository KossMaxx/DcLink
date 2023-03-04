using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Pictures;
using LegacySql.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class PictureRepository : ILegacyPictureRepository
    {
        private readonly LegacyDbContext _db;
        public PictureRepository(LegacyDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<ProductPicture>> GetByProductAsync(int productId, CancellationToken cancellationToken)
        {
            var pictureEf = await _db.ProductPictures.Where(p => p.ProductId == productId).ToListAsync(cancellationToken);
            return pictureEf.Select(p => new ProductPicture(p.Id, p.Url, p.Date));
        }
    }
}
