using System;
using System.Collections.Generic;

namespace LegacySql.Api.Models
{
    public class GetPriceListRequest
    {
        public Guid? ProductManagerId { get; set; }
        public IEnumerable<Guid> ProductTypeIds { get; set; }
        public Guid? ManufacturerId { get; set; }
        public int ColumnId { get; set; }
    }
}