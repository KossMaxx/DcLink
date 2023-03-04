using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Deliveries.Inner
{
    public class DeliveryMethodCarrier : Reference
    {
        public DeliveryMethodCarrier(int id, string title) : base(id, title)
        {
        }
    }
}
