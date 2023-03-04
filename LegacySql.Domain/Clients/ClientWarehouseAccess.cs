using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Clients
{
    public class ClientWarehouseAccess
    {
        public int Id { get; set; }  
        public IdMap WarehouseId { get; set; }
        public bool HasAccess { get; set; }
    }
}
