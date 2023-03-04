using LegacySql.Data;
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
    public class GetEmployeeMappingsQueryHandler : IRequestHandler<GetEmployeeMappingsQuery, IEnumerable<MappingDto>>
    {
        private readonly LegacyDbContext _legacyDb;
        private readonly AppDbContext _appDb;

        public GetEmployeeMappingsQueryHandler(LegacyDbContext legacyDb, AppDbContext appDb)
        {
            _legacyDb = legacyDb;
            _appDb = appDb;
        }

        public async Task<IEnumerable<MappingDto>> Handle(GetEmployeeMappingsQuery query, CancellationToken cancellationToken)
        {
            var mappings = await _appDb.EmployeeMaps
                .Where(m => m.ErpGuid.HasValue)
                .ToListAsync(cancellationToken);

            var titles = await _legacyDb.Employees
                .ToDictionaryAsync(e => e.Id, e => e.FullName, cancellationToken);

            return mappings.Select(m => new MappingDto
            {
                SqlId = m.LegacyId,
                ErpGuid = m.ErpGuid.Value,
                Title = titles[m.LegacyId],
            }).ToList();
        }
    }
}
