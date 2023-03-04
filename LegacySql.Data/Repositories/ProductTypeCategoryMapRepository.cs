using LegacySql.Data.Models;
using LegacySql.Domain.ProductTypeCategories;

namespace LegacySql.Data.Repositories
{
    public class ProductTypeCategoryMapRepository : BaseMapRepository<ProductTypeCategoryMapEF>, IProductTypeCategoryMapRepository
    {
        public ProductTypeCategoryMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
