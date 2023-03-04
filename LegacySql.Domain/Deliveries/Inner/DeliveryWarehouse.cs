namespace LegacySql.Domain.Deliveries.Inner
{
    public class DeliveryWarehouse
    {
        public string Number { get; set; }
        public string Id { get; set; }
        
        public DeliveryWarehouse(string number, string id)
        {
            Number = number;
            Id = id;
        }
    }
}