using LegacySql.Data.Models;
using LegacySql.Domain.Waybills;

namespace LegacySql.Data.Repositories
{
    public class WaybillMapRepository : BaseMapRepository<WaybillMapEF>, IWaybillMapRepository
    {
        public WaybillMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
