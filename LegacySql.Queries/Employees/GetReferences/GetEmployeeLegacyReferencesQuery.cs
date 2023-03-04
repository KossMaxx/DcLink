using LegacySql.Queries.Shared;
using MediatR;
using System.Collections.Generic;

namespace LegacySql.Queries.Employees
{
    public class GetEmployeeLegacyReferencesQuery : IRequest<IEnumerable<LegacyReferenceDto>>
    {
        public GetEmployeeLegacyReferencesQuery(string search)
        {
            Search = search;
        }

        public string Search { get; }
    }
}
