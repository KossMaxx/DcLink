using System;

namespace MessageBus.Clients.Import
{
    public class ErpClientWarehousePriorityDto
    {
        public Guid WarehouseId { get; set; }
        public int Priority { get; set; }
    }
}
