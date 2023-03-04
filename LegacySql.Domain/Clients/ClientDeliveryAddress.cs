namespace LegacySql.Domain.Clients
{
    public class ClientDeliveryAddress
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string WaybillAddress { get; set; }
        public int Type { get; set; }
    }
}
