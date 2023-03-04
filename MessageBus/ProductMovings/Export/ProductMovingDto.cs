using System;
using System.Collections.Generic;

namespace MessageBus.ProductMovings.Export
{
    public class ProductMovingDto
    {
        public int Code { get; set; }
        public DateTime Date { get; set; }
        public string CreatorUsername { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid OutWarehouseId { get; set; }
        public Guid InWarehouseId { get; set; }
        public string Description { get; set; }
        public string Okpo { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsForShipment { get; set; }
        public bool IsShipped { get; set; }
        public IEnumerable<ProductMovingItemDto> Items { get; set; }
    }
}
