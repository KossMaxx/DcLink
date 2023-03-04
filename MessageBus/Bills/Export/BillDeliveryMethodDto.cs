namespace MessageBus.Bills.Export 
{ 
    public class BillDeliveryMethodDto
    {
        public BillDeliveryMethodTypeDto Type { get; set; }
        public BillDeliveryMethodCarrierDto Carrier { get; set; }
    }
}
