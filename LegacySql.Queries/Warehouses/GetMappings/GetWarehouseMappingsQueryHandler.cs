using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Legacy.Data;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Warehouses.GetMappings
{
    public class GetWarehouseMappingsQueryHandler : IRequestHandler<GetWarehouseMappingsQuery, IEnumerable<MappingDto>>
    {
        private readonly LegacyDbContext _legacyDb;
        private readonly AppDbContext _appDb;

        public GetWarehouseMappingsQueryHandler(LegacyDbContext legacyDb, AppDbContext appDb)
        {
            _legacyDb = legacyDb;
            _appDb = appDb;
        }

        public async Task<IEnumerable<MappingDto>> Handle(GetWarehouseMappingsQuery request, CancellationToken cancellationToken)
        {
            var mappings = await _appDb.WarehouseMaps.Where(cb => cb.ErpGuid.HasValue).ToListAsync(cancellationToken);

            var titles = await _legacyDb.Warehouses
                .Where(cb => !string.IsNullOrEmpty(cb.Description))
                .ToDictionaryAsync(e => e.Id, e => e.Description, cancellationToken);

            return mappings.Select(cb => new MappingDto
            {
                SqlId = cb.LegacyId,
                ErpGuid = cb.ErpGuid.Value,
                Title = titles[cb.LegacyId]
            });
        }
    }
}
