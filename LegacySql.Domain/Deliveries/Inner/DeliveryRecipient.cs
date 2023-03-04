namespace LegacySql.Domain.Deliveries.Inner
{
    public class DeliveryRecipient
    {
        public string Name { get; }
        public string Phone { get; }
        public string Email { get; }
        public DeliveryRecipientAddress Address { get; }

        public DeliveryRecipient(string name, string phone, DeliveryRecipientAddress address, string email)
        {
            Name = name;
            Phone = phone;
            Address = address;
            Email = email;
        }
    }
}
