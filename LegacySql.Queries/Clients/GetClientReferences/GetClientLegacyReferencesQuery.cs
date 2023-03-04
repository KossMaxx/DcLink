using System.Collections.Generic;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.Clients.GetClientReferences
{
    public class GetClientLegacyReferencesQuery : IRequest<IEnumerable<LegacyReferenceDto>>
    {
        public GetClientLegacyReferencesQuery(string search)
        {
            Search = search;
        }

        public string Search { get; }
    }
}
