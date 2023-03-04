using LegacySql.Domain.Shared;
using LegacySql.Queries.Diagnostics;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.NotFullMapping.GetNotFullMappingInfo
{
    public class GetNotFullMappingInfoQuery : PagedQuery, IRequest<NotFulMappingStatisticDto>
    {
        public GetNotFullMappingInfoQuery(MappingTypes type, int page, int pageSize) : base(page, pageSize)
        {
            Type = type;
        }

        public MappingTypes Type { get; }
    }
}
