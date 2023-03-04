using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LegacySql.Legacy.Data.Models
{
    [Table("webSkladDopusk")]
    public class ClientWarehouseAccessEF
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public ClientEF Client { get; set; }
        public int WarehouseId { get; set; }
        public WarehouseEF Warehouse { get; set; }
        public bool HasAccess { get; set; }
    }
}
