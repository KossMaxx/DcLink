using System.Collections.Generic;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetErpNotFullMappingCounts
{
    public class GetErpNotFullMappingCountsQuery : IRequest<IEnumerable<NotFullMappingCountsDto>>
    {
    }
}
