using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Legacy.Data;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Warehouses.GetReferences
{
    public class GetWarehouseLegacyReferencesQueryHandler : IRequestHandler<GetWarehouseLegacyReferencesQuery, IEnumerable<LegacyReferenceDto>>
    {
        private readonly LegacyDbContext _db;

        public GetWarehouseLegacyReferencesQueryHandler(LegacyDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LegacyReferenceDto>> Handle(GetWarehouseLegacyReferencesQuery request, CancellationToken cancellationToken)
        {
            var warehouseesQuery = _db.Warehouses.AsQueryable();
            if (!string.IsNullOrEmpty(request.Search))
            {
                warehouseesQuery = warehouseesQuery.Where(e => e.Description.Contains(request.Search));
            }

            return await warehouseesQuery
                .Select(e => new LegacyReferenceDto
                {
                    Id = e.Id,
                    Title = e.Description,
                })
                .ToListAsync(cancellationToken);
        }
    }
}