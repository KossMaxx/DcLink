using System;

namespace MessageBus.Bills.Export
{
    public class BillDeliveryRecipientAddressDto
    {
        public string Title { get; set; }
        public string City { get; set; }
        public Guid? CityId { get; set; }
    }
}
