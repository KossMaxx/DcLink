using System.Collections.Generic;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.Clients.GetClientMappings
{
    public class GetClientMappingsQuery : IRequest<IEnumerable<MappingDto>>
    {
    }
}
