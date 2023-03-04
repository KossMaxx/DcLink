using System.Collections.Generic;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetTemporaryMappingCounts
{
    public class GetTemporaryMappingCountsQuery : IRequest<IEnumerable<NotFullMappingCountsDto>>
    {
    }
}
