using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Legacy.Data;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.ProductTypes.GetMappings
{
    public class GetProductTypeMappingsQueryHandler : IRequestHandler<GetProductTypeMappingsQuery, IEnumerable<MappingDto>>
    {
        private readonly LegacyDbContext _legacyDb;
        private readonly AppDbContext _appDb;

        public GetProductTypeMappingsQueryHandler(LegacyDbContext legacyDb, AppDbContext appDb)
        {
            _legacyDb = legacyDb;
            _appDb = appDb;
        }

        public async Task<IEnumerable<MappingDto>> Handle(GetProductTypeMappingsQuery request, CancellationToken cancellationToken)
        {
            var mappings = await _appDb.ProductTypeMaps.Where(e => e.ErpGuid.HasValue).ToListAsync(cancellationToken);

            var titles = await _legacyDb.ProductTypes
                .ToDictionaryAsync(e => e.Code, e => e.FullName, cancellationToken);

            return mappings.Select(e => new MappingDto
            {
                SqlId = e.LegacyId,
                ErpGuid = e.ErpGuid.Value,
                Title = titles[e.LegacyId]
            });
        }
    }
}
