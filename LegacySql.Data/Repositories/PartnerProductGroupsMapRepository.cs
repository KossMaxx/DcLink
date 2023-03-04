using LegacySql.Data.Models;
using LegacySql.Domain.PartnerProductGroups;

namespace LegacySql.Data.Repositories
{
    public class PartnerProductGroupsMapRepository : BaseMapRepository<PartnerProductGroupMapEF>, IPartnerProductGroupsMapRepository
    {
        public PartnerProductGroupsMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
