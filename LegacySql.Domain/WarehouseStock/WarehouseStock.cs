using LegacySql.Domain.Shared;

namespace LegacySql.Domain.WarehouseStock
{
    public class WarehouseStock
    {
        public IdMap WarehouseId { get; }
        public int Quantity { get; }

        public WarehouseStock(IdMap warehouseId, int quantity)
        {
            WarehouseId = warehouseId;
            Quantity = quantity;
        }
    }
}
