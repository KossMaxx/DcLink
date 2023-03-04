using System;

namespace LegacySql.Api.Models
{
    public class GetIndividualPriceListRequest
    {
        public Guid ClientId { get; set; }
        public Guid? ProductTypeId { get; set; }
        public Guid? ManufacturerId { get; set; }
        public Guid? ProductManagerId { get; set; }
    }
}
