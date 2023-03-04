using LegacySql.Data.Models;
using LegacySql.Domain.SegmentationTurnovers;

namespace LegacySql.Data.Repositories
{
    public class SegmentationTurnoversMapRepository : BaseMapRepository<SegmentationTurnoverMapEF>, ISegmentationTurnoversMapRepository
    {
        public SegmentationTurnoversMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
