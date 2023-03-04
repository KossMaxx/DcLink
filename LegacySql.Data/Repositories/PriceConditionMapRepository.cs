using LegacySql.Data.Models;
using LegacySql.Domain.PriceConditions;

namespace LegacySql.Data.Repositories
{
    public class PriceConditionMapRepository : BaseMapRepository<PriceConditionMapEF>, IPriceConditionMapRepository
    {
        public PriceConditionMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
