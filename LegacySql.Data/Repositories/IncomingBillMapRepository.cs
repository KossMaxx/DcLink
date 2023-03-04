using LegacySql.Data.Models;
using LegacySql.Domain.IncomingBills;

namespace LegacySql.Data.Repositories
{
    public class IncomingBillMapRepository : BaseMapRepository<IncomingBillMapEF>, IIncomingBillMapRepository
    {
        public IncomingBillMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
