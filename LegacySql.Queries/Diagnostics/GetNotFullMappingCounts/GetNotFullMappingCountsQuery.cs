using System.Collections.Generic;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetNotFullMappingCounts
{
    public class GetNotFullMappingCountsQuery : IRequest<IEnumerable<NotFullMappingCountsDto>>
    {
    }
}
