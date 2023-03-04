using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Legacy.Data.Clients
{
    internal class WarehouseAccessData
    {
        public int ClientWarehouseAccess_Id { get; set; }
        public int ClientWarehouseAccess_ClientId { get; set; }
        public int ClientWarehouseAccess_WarehouseId { get; set; }
        public bool ClientWarehouseAccess_HasAccess { get; set; }
    }
}
