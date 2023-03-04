using LegacySql.Data.Models;
using LegacySql.Domain.ProductTypeCategoryParameters;

namespace LegacySql.Data.Repositories
{
    public class ProductTypeCategoryParameterMapRepository : BaseMapRepository<ProductTypeCategoryParameterMapEF>, IProductTypeCategoryParameterMapRepository
    {
        public ProductTypeCategoryParameterMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
