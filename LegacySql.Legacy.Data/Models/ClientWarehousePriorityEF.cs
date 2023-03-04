using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_Client_Sklad_Reserv_Priority")]
    public class ClientWarehousePriorityEF
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public ClientEF Client { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseEF Warehouse { get; set; }
        public int Priority { get; set; }
    }
}
