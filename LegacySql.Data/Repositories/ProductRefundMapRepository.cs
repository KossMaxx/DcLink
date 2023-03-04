using LegacySql.Data.Models;
using LegacySql.Domain.ProductRefunds;

namespace LegacySql.Data.Repositories
{
    public class ProductRefundMapRepository : BaseMapRepository<ProductRefundMapEF>, IProductRefundMapRepository
    {
        public ProductRefundMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
