using LegacySql.Domain.Shared;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetErpNotFullMappingInfo
{
    public class GetErpNotFullMappingInfoQuery : PagedQuery, IRequest<ErpNotFulMappingStatisticDto>
    {
        public GetErpNotFullMappingInfoQuery(MappingTypes type, int page, int pageSize) : base(page, pageSize)
        {
            Type = type;
        }

        public MappingTypes Type { get; }
    }
}
