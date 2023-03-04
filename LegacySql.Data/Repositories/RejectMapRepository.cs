using LegacySql.Data.Models;
using LegacySql.Domain.Rejects;

namespace LegacySql.Data.Repositories
{
    public class RejectMapRepository : BaseMapRepository<RejectMapEF>, IRejectMapRepository
    {
        public RejectMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
