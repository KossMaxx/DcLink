using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Legacy.Data.Clients
{
    internal class WarehousePriorityData
    {
        public int ClientWarehousePriority_Id { get; set; }
        public int ClientWarehousePriority_ClientId { get; set; }
        public int ClientWarehousePriority_WarehouseId { get; set; }
        public int ClientWarehousePriority_Priority { get; set; }
    }
}
