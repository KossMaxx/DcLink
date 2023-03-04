using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Diagnostics.GetNotFullMappingCounts
{
    public class GetNotFullMappingCountsQueryHandler : IRequestHandler<GetNotFullMappingCountsQuery, IEnumerable<NotFullMappingCountsDto>>
    {
        private readonly AppDbContext _appDb;

        public GetNotFullMappingCountsQueryHandler(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task<IEnumerable<NotFullMappingCountsDto>> Handle(GetNotFullMappingCountsQuery request, CancellationToken cancellationToken)
        {
            return await _appDb.NotFullMapped.GroupBy(e => e.Type)
                .Select(e=>new NotFullMappingCountsDto
                {
                    EntityType = e.Key.ToString(),
                    Quantity = e.Count()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
