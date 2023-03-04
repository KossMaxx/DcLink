namespace MessageBus.ClientOrder.Export 
{ 
    public class OrderDeliveryMethodDto
    {
        public OrderDeliveryMethodTypeDto Type { get; set; }
        public OrderDeliveryMethodCarrierDto Carrier { get; set; }
    }
}
