using System.Collections.Generic;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.Warehouses.GetMappings
{
    public class GetWarehouseMappingsQuery : IRequest<IEnumerable<MappingDto>> { }
}