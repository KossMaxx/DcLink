using LegacySql.Data.Models;
using LegacySql.Domain.ProductMoving;

namespace LegacySql.Data.Repositories
{
    public class ProductMovingMapRepository : BaseMapRepository<ProductMovingMapEF>, IProductMovingMapRepository
    {
        public ProductMovingMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
