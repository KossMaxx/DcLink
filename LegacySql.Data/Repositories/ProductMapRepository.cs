using LegacySql.Data.Models;
using LegacySql.Domain.Products;

namespace LegacySql.Data.Repositories
{
    public class ProductMapRepository : BaseMapRepository<ProductMapEF>, IProductMapRepository
    {
        public ProductMapRepository(AppDbContext db) : base(db)
        { }
    }
}
