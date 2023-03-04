using LegacySql.Data.Models;
using LegacySql.Domain.Cashboxes;

namespace LegacySql.Data.Repositories
{
    public class CashboxPaymentMapRepository : BaseMapRepository<CashboxPaymentMapEF>, ICashboxPaymentMapRepository
    {
        public CashboxPaymentMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
