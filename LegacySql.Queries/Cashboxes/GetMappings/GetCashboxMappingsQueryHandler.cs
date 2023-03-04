using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Legacy.Data;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Cashboxes.GetMappings
{
    public class GetCashboxMappingsQueryHandler : IRequestHandler<GetCashboxMappingsQuery, IEnumerable<CashboxMappingDto>>
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _appDb;

        public GetCashboxMappingsQueryHandler(LegacyDbContext db, AppDbContext appDb)
        {
            _db = db;
            _appDb = appDb;
        }

        public async Task<IEnumerable<CashboxMappingDto>> Handle(GetCashboxMappingsQuery request, CancellationToken cancellationToken)
        {
            var mappings = await _appDb.CashboxMaps.Where(cb => cb.ErpGuid.HasValue).ToListAsync(cancellationToken);
            
            if (mappings.Any(e => string.IsNullOrEmpty(e.Description)))
            {
                await FillDescriptions(cancellationToken);
            }

            return mappings.Select(cb => new CashboxMappingDto
            {
                SqlId = cb.LegacyId,
                ErpGuid = cb.ErpGuid.Value,
                Description = cb.Description,
            });
        }

        private async Task FillDescriptions(CancellationToken cancellationToken)
        {
            var mappingsWithoutTitle = await _appDb.CashboxMaps
                .Where(cb => string.IsNullOrEmpty(cb.Description)).ToListAsync(cancellationToken);
            foreach (var mapping in mappingsWithoutTitle)
            {
                var legacyType = await _db.Cashboxes.FirstOrDefaultAsync(e => e.Id == mapping.LegacyId, cancellationToken);
                mapping.Description = legacyType.Description;
            }

            await _appDb.SaveChangesAsync(cancellationToken);
        }
    }
}
