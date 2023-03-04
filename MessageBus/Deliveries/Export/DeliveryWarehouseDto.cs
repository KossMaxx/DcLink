using System;

namespace MessageBus.Deliveries.Export
{
    public class DeliveryWarehouseDto
    {
        public Guid? ClientOrderId { get; set; }
        public int? TypeId { get; set; }
    }
}
