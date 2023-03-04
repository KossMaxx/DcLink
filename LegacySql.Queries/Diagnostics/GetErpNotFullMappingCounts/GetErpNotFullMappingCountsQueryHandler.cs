using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Diagnostics.GetErpNotFullMappingCounts
{
    public class GetErpNotFullMappingCountsQueryHandler : IRequestHandler<GetErpNotFullMappingCountsQuery, IEnumerable<NotFullMappingCountsDto>>
    {
        private readonly AppDbContext _appDb;

        public GetErpNotFullMappingCountsQueryHandler(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task<IEnumerable<NotFullMappingCountsDto>> Handle(GetErpNotFullMappingCountsQuery request, CancellationToken cancellationToken)
        {
            return await _appDb.ErpNotFullMapped.GroupBy(e => e.Type)
                .Select(e => new NotFullMappingCountsDto
                {
                    EntityType = e.Key.ToString(),
                    Quantity = e.Count()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
