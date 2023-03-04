using System;
using System.Collections.Generic;

namespace MessageBus.ProductMovings.Import
{
    public class ErpProductMovingDto
    {
        public Guid Id { get; set; }
        public int Code { get; set; }
        public DateTime Date { get; set; }
        public string CreatorUsername { get; set; }
        public Guid OutWarehouseId { get; set; }
        public Guid InWarehouseId { get; set; }
        public string Description { get; set; }
        public string Okpo { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsForShipment { get; set; }
        public bool IsShipped { get; set; }
        public IEnumerable<ErpProductMovingItemDto> Items { get; set; }
    }
}
