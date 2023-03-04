using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Queries.Infrastructure;
using LegacySql.Queries.NotFullMapping;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetErpNotFullMappingInfo
{
    public class GetErpNotFullMappingInfoQueryHandler : IRequestHandler<GetErpNotFullMappingInfoQuery, ErpNotFulMappingStatisticDto>
    {
        private readonly AppDbContext _mapDb;

        public GetErpNotFullMappingInfoQueryHandler(AppDbContext mapDb)
        {
            _mapDb = mapDb;
        }

        public async Task<ErpNotFulMappingStatisticDto> Handle(GetErpNotFullMappingInfoQuery request, CancellationToken cancellationToken)
        {
            var result = await _mapDb.ErpNotFullMapped.Where(e => e.Type == request.Type).OrderByDescending(e => e.Date).GetPageAsync(request.Page, request.PageSize);

            return new ErpNotFulMappingStatisticDto
            {
                Page = request.Page,
                Total = result.totalItems,
                Items = result.items.Select(e => new ErpNotFulMappingStatisticItem
                {
                    Id = e.ErpId,
                    Date = e.Date.ToString("dd.MM.yyyy HH:mm"),
                    Reasons = string.IsNullOrEmpty(e.Why) ? (IEnumerable<string>)new List<string>() : e.Why.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                })
            };
        }
    }
}
