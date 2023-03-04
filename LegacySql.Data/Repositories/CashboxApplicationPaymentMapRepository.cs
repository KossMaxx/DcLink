using LegacySql.Data.Models;
using LegacySql.Domain.Cashboxes;

namespace LegacySql.Data.Repositories
{
    public class CashboxApplicationPaymentMapRepository : BaseMapRepository<CashboxApplicationPaymentMapEF>, ICashboxApplicationPaymentMapRepository
    {
        public CashboxApplicationPaymentMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
