using LegacySql.Data.Models;
using LegacySql.Domain.PhysicalPersons;

namespace LegacySql.Data.Repositories
{
    public class PhysicalPersonMapRepository : BaseMapRepository<PhysicalPersonMapEF>, IPhysicalPersonMapRepository
    {
        public PhysicalPersonMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
