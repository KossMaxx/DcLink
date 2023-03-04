using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Deliveries.PublishEntity
{
    public class DeliveryWarehouse
    {
        public IdMap ClientOrderId { get; }
        public int? TypeId { get; }
        public DeliveryWarehouse(IdMap clientOrderId, int? typeId)
        {
            ClientOrderId = clientOrderId;
            TypeId = typeId;
        }
    }
}
