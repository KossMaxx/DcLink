using System.Collections.Generic;
using LegacySql.Domain.Shared;
using LegacySql.Queries.NotFullMapping;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetTemporaryMappingInfo
{
    public class GetTemporaryMappingInfoQuery : PagedQuery, IRequest<TemporaryMappingStatisticDto>
    {
        public GetTemporaryMappingInfoQuery(MappingTypes type, int page, int pageSize) : base(page, pageSize)
        {
            Type = type;
        }

        public MappingTypes Type { get; }
    }
}
