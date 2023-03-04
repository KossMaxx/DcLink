namespace MessageBus.Bills.Export
{
    public class BillDeliveryDto
    {
        public BillDeliveryMethodDto Method { get; set; }
        public BillDeliveryRecipientDto Recipient { get; set; }
        public float? Weight { get; set; }
        public float? Volume { get; set; }
        public decimal? DeclaredPrice { get; set; }
        public string PayerType { get; set; }
        public string PaymentMethod { get; set; }
        public string CargoType { get; set; }
        public string ServiceType { get; set; }
        public bool CashOnDelivery { get; set; }
        public BillDeliveryWarehouseDto Warehouse { get; set; }
        public string CargoInvoice { get; set; }
    }
}
