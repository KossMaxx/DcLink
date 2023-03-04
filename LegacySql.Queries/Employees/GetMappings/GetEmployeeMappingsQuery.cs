using LegacySql.Queries.Shared;
using MediatR;
using System.Collections.Generic;

namespace LegacySql.Queries.Employees
{
    public class GetEmployeeMappingsQuery: IRequest<IEnumerable<MappingDto>>
    {
    }
}
