using System.Collections.Generic;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.Warehouses.GetReferences
{
    public class GetWarehouseLegacyReferencesQuery : IRequest<IEnumerable<LegacyReferenceDto>>
    {
        public GetWarehouseLegacyReferencesQuery(string search)
        {
            Search = search;
        }

        public string Search { get; }
    }
}
