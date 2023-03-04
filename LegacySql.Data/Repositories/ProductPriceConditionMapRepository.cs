using LegacySql.Data.Models;
using LegacySql.Domain.ProductPriceConditions;

namespace LegacySql.Data.Repositories
{
    public class ProductPriceConditionMapRepository : BaseMapRepository<ProductPriceConditionMapEF>, IProductPriceConditionMapRepository
    {
        public ProductPriceConditionMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
