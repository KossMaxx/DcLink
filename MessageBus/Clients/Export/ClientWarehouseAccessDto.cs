using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBus.Clients.Export
{
    public class ClientWarehouseAccessDto
    {
        public int Id { get; set; }
        public Guid WarehouseId { get; set; }
        public bool HasAccess { get; set; }
    }
}
