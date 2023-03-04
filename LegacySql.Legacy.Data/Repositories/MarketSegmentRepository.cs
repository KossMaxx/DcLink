using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.MarketSegments;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;

namespace LegacySql.Legacy.Data.Repositories
{
    public class MarketSegmentRepository : ILegacyMarketSegmentRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        public MarketSegmentRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        public async Task<IEnumerable<MarketSegment>> GetAllAsync(CancellationToken cancellationToken)
        {
            var segmentsEf = await _db.MarketSegments
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            return segmentsEf.Select(async i => await MapToDomain(i, cancellationToken)).Select(i => i.Result);
        }

        private async Task<MarketSegment> MapToDomain(MarketSegmentEF segmentEf, CancellationToken cancellationToken)
        {
            var segmentMap = await _mapDb.MarketSegmentMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == segmentEf.Id, cancellationToken: cancellationToken);
            
            return new MarketSegment
            (
                new IdMap(segmentEf.Id, segmentMap?.ErpGuid),
                segmentEf.Title,
                segmentMap != null
            );
        }
    }
}

