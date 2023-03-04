using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Legacy.Data;
using LegacySql.Queries.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.ProductTypes.GetProductTypeLegacyReferences
{
    public class GetProductTypeLegacyReferencesQueryHandler : IRequestHandler<GetProductTypeLegacyReferencesQuery, IEnumerable<LegacyReferenceDto>>
    {
        private readonly LegacyDbContext _db;

        public GetProductTypeLegacyReferencesQueryHandler(LegacyDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LegacyReferenceDto>> Handle(GetProductTypeLegacyReferencesQuery request, CancellationToken cancellationToken)
        {
            var query = _db.ProductTypes.Where(t => t.Status != 1);
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(e => e.Name.ToUpper().Contains(request.Search.ToUpper()));
            }

            return await query.Select(e => new LegacyReferenceDto
                {
                    Id = e.Code,
                    Title = e.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
