using System.Collections.Generic;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.ProductTypes.GetMappings
{
    public class GetProductTypeMappingsQuery : IRequest<IEnumerable<MappingDto>>
    {}
}
