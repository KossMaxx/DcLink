using LegacySql.Data.Models;
using LegacySql.Domain.Bills;

namespace LegacySql.Data.Repositories
{
    public class BillMapRepository : BaseMapRepository<BillMapEF>, IBillMapRepository
    {
        public BillMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
