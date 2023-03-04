using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBus.Clients.Export
{
    public class ClientWarehousePriorityDto
    {
        public int Id { get; set; }
        public Guid WarehouseId { get; set; }
        public int Priority { get; set; }
    }
}
