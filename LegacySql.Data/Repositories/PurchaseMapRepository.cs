using LegacySql.Data.Models;
using LegacySql.Domain.Purchases;

namespace LegacySql.Data.Repositories
{
    public class PurchaseMapRepository : BaseMapRepository<PurchaseMapEF>, IPurchaseMapRepository
    {
        public PurchaseMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
