using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Legacy.Data;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Clients.GetClientMappings
{
    public class GetClientMappingsQueryHandler : IRequestHandler<GetClientMappingsQuery, IEnumerable<MappingDto>>
    {
        private readonly LegacyDbContext _legacyDb;
        private readonly AppDbContext _appDb;

        public GetClientMappingsQueryHandler(LegacyDbContext legacyDb, AppDbContext appDb)
        {
            _legacyDb = legacyDb;
            _appDb = appDb;
        }

        public async Task<IEnumerable<MappingDto>> Handle(GetClientMappingsQuery request, CancellationToken cancellationToken)
        {
            var mappings = await _appDb.ClientMaps.Where(e => e.ErpGuid.HasValue).ToListAsync(cancellationToken);

            var titles = await _legacyDb.Clients
                .ToDictionaryAsync(e => e.Id, e => e.Title, cancellationToken);

            return mappings.Select(e => new MappingDto
            {
                SqlId = e.LegacyId,
                ErpGuid = e.ErpGuid.Value,
                Title = titles.ContainsKey(e.LegacyId) ? titles[e.LegacyId] : string.Empty
            });
        }
    }
}
