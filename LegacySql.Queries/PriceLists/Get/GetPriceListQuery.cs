using System;
using System.Collections.Generic;
using LegacySql.Domain.SellingPrices;
using MediatR;

namespace LegacySql.Queries.PriceLists.Get
{
    public class GetPriceListQuery : IRequest<IEnumerable<PriceListItemDto>>
    {
        public SellingPriceColumn PriceColumn { get; set; }
        public IEnumerable<Guid> ProductTypeIds { get; set; }
        public Guid? ProductManagerId { get; set; }
        public Guid? ManufacturerId { get; set; }
    }
}
