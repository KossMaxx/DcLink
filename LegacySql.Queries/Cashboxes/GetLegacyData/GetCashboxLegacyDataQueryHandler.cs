using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Legacy.Data;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Cashboxes.GetLegacyData
{
    public class GetCashboxLegacyDataQueryHandler : IRequestHandler<GetCashboxLegacyDataQuery, IEnumerable<LegacyReferenceDto>>
    {
        private readonly LegacyDbContext _db;

        public GetCashboxLegacyDataQueryHandler(LegacyDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LegacyReferenceDto>> Handle(GetCashboxLegacyDataQuery request, CancellationToken cancellationToken)
        {
            var cashboxesQuery = _db.Cashboxes.AsQueryable();
            if (!string.IsNullOrEmpty(request.Search))
            {
                cashboxesQuery = cashboxesQuery.Where(e => e.Description.Contains(request.Search));
            }

            return await cashboxesQuery
                .Select(e => new LegacyReferenceDto
                {
                    Id = e.Id,
                    Title = e.Description,
                })
                .ToListAsync(cancellationToken);
        }
    }
}