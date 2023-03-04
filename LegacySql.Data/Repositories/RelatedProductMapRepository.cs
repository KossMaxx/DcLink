using LegacySql.Data.Models;
using LegacySql.Domain.RelatedProducts;

namespace LegacySql.Data.Repositories
{
    public class RelatedProductMapRepository : BaseMapRepository<RelatedProductMapEF>, IRelatedProductMapRepository
    {
        public RelatedProductMapRepository(AppDbContext db) : base(db)
        { }
    }
}
