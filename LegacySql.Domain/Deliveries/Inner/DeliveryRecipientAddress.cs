using System;

namespace LegacySql.Domain.Deliveries.Inner
{
    public class DeliveryRecipientAddress
    {
        public string Title { get; }
        public string City { get; }
        public Guid? CityId { get; }

        public DeliveryRecipientAddress(string title, string city, Guid? cityId)
        {
            Title = title;
            City = city;
            CityId = cityId;
        }
    }
}
