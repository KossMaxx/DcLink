using LegacySql.Data.Models;
using LegacySql.Domain.Firms;

namespace LegacySql.Data.Repositories
{
    public class FirmMapRepository : BaseMapRepository<FirmMapEF>, IFirmMapRepository
    {
        public FirmMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
