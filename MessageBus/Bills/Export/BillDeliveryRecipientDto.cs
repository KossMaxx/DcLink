namespace MessageBus.Bills.Export
{
    public class BillDeliveryRecipientDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public BillDeliveryRecipientAddressDto Address { get; set; }
    }
}
