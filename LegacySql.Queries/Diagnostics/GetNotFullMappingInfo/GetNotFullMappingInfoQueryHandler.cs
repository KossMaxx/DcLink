using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Queries.Diagnostics;
using LegacySql.Queries.Infrastructure;
using MediatR;

namespace LegacySql.Queries.NotFullMapping.GetNotFullMappingInfo
{
    public class GetNotFullMappingInfoQueryHandler : IRequestHandler<GetNotFullMappingInfoQuery, NotFulMappingStatisticDto>
    {
        private readonly AppDbContext _mapDb;

        public GetNotFullMappingInfoQueryHandler(AppDbContext mapDb)
        {
            _mapDb = mapDb;
        }

        public async Task<NotFulMappingStatisticDto> Handle(GetNotFullMappingInfoQuery request, CancellationToken cancellationToken)
        {
            var result = await _mapDb.NotFullMapped.Where(e => e.Type == request.Type).OrderByDescending(e => e.Date).GetPageAsync(request.Page,request.PageSize);

            return new NotFulMappingStatisticDto
            {
                Page = request.Page,
                Total = result.totalItems,
                Items = result.items.Select(e=>new NotFulMappingStatisticItem
                {
                    Id = e.InnerId,
                    Date = e.Date.ToString("dd.MM.yyyy HH:mm"),
                    Reasons = string.IsNullOrEmpty(e.Why) ? (IEnumerable<string>) new List<string>() : e.Why.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                })
            };
        }
    }
}
