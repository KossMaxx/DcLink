using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Deliveries.Inner
{
    public class DeliveryMethodType : Reference
    {
        public DeliveryMethodType(int id, string title) : base(id, title)
        {
        }
    }
}
