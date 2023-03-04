using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Clients
{
    public class ClientWarehousePriority
    {
        public int Id { get; set; }
        public IdMap WarehouseId { get; set; }
        public int Priority { get; set; }
    }
}
