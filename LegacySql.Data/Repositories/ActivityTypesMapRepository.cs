using LegacySql.Data.Models;
using LegacySql.Domain.ActivityTypes;

namespace LegacySql.Data.Repositories
{
    public class ActivityTypesMapRepository : BaseMapRepository<ActivityTypeMapEF>, IActivityTypesMapRepository
    {
        public ActivityTypesMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
