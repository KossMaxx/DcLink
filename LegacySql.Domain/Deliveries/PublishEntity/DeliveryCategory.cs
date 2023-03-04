namespace LegacySql.Domain.Deliveries.PublishEntity
{
    public class DeliveryCategory
    {
        public int Id { get; }
        public string Title { get; }

        public DeliveryCategory(int id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
