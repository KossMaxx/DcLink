using LegacySql.Data.Models;
using LegacySql.Domain.MarketSegments;

namespace LegacySql.Data.Repositories
{
    public class MarketSegmentMapRepository : BaseMapRepository<MarketSegmentMapEF>, IMarketSegmentMapRepository
    {
        public MarketSegmentMapRepository(AppDbContext db) : base(db)
        {}
    }
}
