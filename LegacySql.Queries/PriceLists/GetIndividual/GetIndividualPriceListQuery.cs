using System;
using System.Collections.Generic;
using MediatR;

namespace LegacySql.Queries.PriceLists.GetIndividual
{
    public class GetIndividualPriceListQuery : IRequest<IEnumerable<IndividualPriceListItemDto>>
    {
        public Guid ClientId { get; set; }
        public Guid? ProductTypeId { get; set; }
        public Guid? ManufacturerId { get; set; }
        public Guid? ProductManagerId { get; set; }
    }
}
