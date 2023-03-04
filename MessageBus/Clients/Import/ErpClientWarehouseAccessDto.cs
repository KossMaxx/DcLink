using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBus.Clients.Import
{
    public class ErpClientWarehouseAccessDto
    {
        public Guid WarehouseId { get; set; }
        public bool HasAccess { get; set; }
    }
}
