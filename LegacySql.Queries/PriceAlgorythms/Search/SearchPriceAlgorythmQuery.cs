using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Queries.PriceAlgorythms.Search
{
    public class SearchPriceAlgorythmQuery : IRequest<IEnumerable<PriceAlgorythmReferenceDto>>
    {
        public string SearchTerm { get; set; }
    }
}
