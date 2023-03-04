using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Legacy.Data;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Clients.GetClientReferences
{
    public class GetClientLegacyReferencesQueryHandler : IRequestHandler<GetClientLegacyReferencesQuery, IEnumerable<LegacyReferenceDto>>
    {
        private readonly LegacyDbContext _legacyDb;

        public GetClientLegacyReferencesQueryHandler(LegacyDbContext legacyDb)
        {
            _legacyDb = legacyDb;
        }

        public async Task<IEnumerable<LegacyReferenceDto>> Handle(GetClientLegacyReferencesQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _legacyDb.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(query.Search))
            {
                dbQuery = dbQuery.Where(e => e.Title.ToUpper().Contains(query.Search.ToUpper()));
            }

            var references = await dbQuery.Select(e => new LegacyReferenceDto(e.Id, e.Title))
                .ToListAsync(cancellationToken);

            return references;
        }
    }
}
