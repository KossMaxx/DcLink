using System.Collections.Generic;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.ProductTypes.GetProductTypeLegacyReferences
{
    public class GetProductTypeLegacyReferencesQuery : IRequest<IEnumerable<LegacyReferenceDto>>
    {
        public GetProductTypeLegacyReferencesQuery(string search)
        {
            Search = search;
        }

        public string Search { get; }
    }
}
