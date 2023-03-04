using MediatR;
using System.Collections.Generic;

namespace LegacySql.Queries.PriceLists.GetUnregistered
{
    public class GetUnregisteredProductPricesQuery : IRequest<IEnumerable<UnregisteredProductPriceDto>>
    {
        public string VendorCode { get; set; }
        public string ClientTitle { get; set; }
    }
}
