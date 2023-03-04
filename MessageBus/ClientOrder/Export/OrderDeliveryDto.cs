namespace MessageBus.ClientOrder.Export
{
    public class OrderDeliveryDto
    {
        public OrderDeliveryMethodDto Method { get; set; }
        public OrderDeliveryRecipientDto Recipient { get; set; }
        public float? Weight { get; set; }
        public float? Volume { get; set; }
        public decimal? DeclaredPrice { get; set; }
        public string PayerType { get; set; }
        public string PaymentMethod { get; set; }
        public string CargoType { get; set; }
        public string ServiceType { get; set; }
        public bool CashOnDelivery { get; set; }
        public OrderDeliveryWarehouseDto Warehouse { get; set; }
        public string CargoInvoice { get; set; }
    }
}
