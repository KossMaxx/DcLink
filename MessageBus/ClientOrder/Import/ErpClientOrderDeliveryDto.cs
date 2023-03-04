using System;

namespace MessageBus.ClientOrder.Import
{
    public class ErpClientOrderDeliveryDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientEmail { get; set; }
        public Guid? RecipientCityId { get; set; }
        public float? Weight { get; set; }
        public float? Volume { get; set; }
        public decimal? DeclaredPrice { get; set; }
        public string PaymentMethod { get; set; }
        public bool CashOnDelivery { get; set; }
        public string CargoInvoice { get; set; }
    }
}
