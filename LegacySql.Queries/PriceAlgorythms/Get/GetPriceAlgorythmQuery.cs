using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Queries.PriceAlgorythms.Get
{
    public class GetPriceAlgorythmQuery : IRequest<PriceAlgorythmDto>
    {
        public int Id { get; set; }
    }
}
