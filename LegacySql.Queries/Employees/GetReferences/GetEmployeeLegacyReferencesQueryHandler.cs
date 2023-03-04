using LegacySql.Legacy.Data;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Queries.Employees
{
    public class GetEmployeeLegacyReferencesQueryHandler : IRequestHandler<GetEmployeeLegacyReferencesQuery, IEnumerable<LegacyReferenceDto>>
    {
        private readonly LegacyDbContext _legacyDb;

        public GetEmployeeLegacyReferencesQueryHandler(LegacyDbContext legacyDb)
        {
            _legacyDb = legacyDb;
        }

        public async Task<IEnumerable<LegacyReferenceDto>> Handle(GetEmployeeLegacyReferencesQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _legacyDb.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(query.Search))
            {
                dbQuery = dbQuery.Where(e => e.FullName.ToUpper().Contains(query.Search.ToUpper()));
            }

            var references = await dbQuery.Select(e => new LegacyReferenceDto(e.Id, e.FullName))
                .ToListAsync(cancellationToken);

            return references;
        }
    }
}
