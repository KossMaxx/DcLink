namespace MessageBus.ClientOrder.Export
{
    public class OrderDeliveryRecipientDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public OrderDeliveryRecipientAddressDto Address { get; set; }
    }
}
