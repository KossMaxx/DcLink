using System.Collections.Generic;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.Cashboxes.GetLegacyData
{
    public class GetCashboxLegacyDataQuery : IRequest<IEnumerable<LegacyReferenceDto>>
    {
        public GetCashboxLegacyDataQuery(string search)
        {
            Search = search;
        }

        public string Search { get; }
    }
}
