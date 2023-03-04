using System.Collections.Generic;
using MediatR;

namespace LegacySql.Queries.PriceAlgorythms.PreliminaryPrices
{
    public class GetPreliminaryPricesQuery : IRequest<IEnumerable<PreliminaryPriceAlgorythmDto>>
    {
        public int Id { get; set; }
    }
}
