using LegacySql.Data.Models;
using LegacySql.Domain.ReconciliationActs;

namespace LegacySql.Data.Repositories
{
    public class ReconciliationActMapRepository : BaseMapRepository<ReconciliationActMapEF>, IReconciliationActMapRepository
    {
        public ReconciliationActMapRepository(AppDbContext db) : base(db) { }
    }
}