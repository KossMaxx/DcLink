using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductDescriptions;
using LegacySql.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ProductDescriptionRepository : ILegacyProductDescriptionRepository
    {
        private readonly LegacyDbContext _db;

        public ProductDescriptionRepository(LegacyDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProductDescription>> GetByProductAsync(int productId, CancellationToken cancellationToken)
        {
            var descriptionEf = await _db.ProductDescriptions.Where(d => d.ProductId == productId).ToListAsync(cancellationToken);
            return descriptionEf.Select(d => new ProductDescription(
                d.Id, 
                new ProductId(d.ProductId), 
                new LanguageId(d.LanguageId), 
                d.Description, 
                d.Uuu, 
                d.Date)
            );
        }
    }
}
