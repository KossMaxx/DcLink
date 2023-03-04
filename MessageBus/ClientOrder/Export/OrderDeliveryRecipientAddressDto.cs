using System;

namespace MessageBus.ClientOrder.Export
{
    public class OrderDeliveryRecipientAddressDto
    {
        public string Title { get; set; }
        public string City { get; set; }
        public Guid? CityId { get; set; }
    }
}
