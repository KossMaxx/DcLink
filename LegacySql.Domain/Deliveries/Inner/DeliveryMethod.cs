using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Deliveries.Inner
{
    public class DeliveryMethod
    {
        public DeliveryMethodType Type { get; }
        public DeliveryMethodCarrier Carrier { get; }
        public DeliveryMethod(DeliveryMethodType type, DeliveryMethodCarrier carrier)
        {
            Type = type;
            Carrier = carrier;
        }
    }
}
