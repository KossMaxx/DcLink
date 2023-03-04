using System;

namespace LegacySql.Domain.Deliveries.Inner
{
    public class Delivery
    {
        public DeliveryMethod Method { get; }
        public DeliveryRecipient Recipient { get; }
        public float? Weight { get; }
        public float? Volume { get; }
        public decimal? DeclaredPrice { get; }
        public string PayerType { get; }
        public string PaymentMethod { get; }
        public string CargoType { get; }
        public string ServiceType { get; }
        public bool CashOnDelivery { get; }
        public DeliveryWarehouse Warehouse { get; }
        public string CargoInvoice { get; }
        public DateTime? ChangedAt { get; }

        public Delivery(DeliveryMethod method, DeliveryRecipient recipient, float? weight, float? volume, decimal? declaredPrice, string payerType, string paymentMethod, string cargoType, string serviceType, bool cashOnDelivery, DeliveryWarehouse warehouse, string cargoInvoice, DateTime? changedAt)
        {
            Method = method;
            Recipient = recipient;
            Weight = weight;
            Volume = volume;
            DeclaredPrice = declaredPrice;
            PayerType = payerType;
            PaymentMethod = paymentMethod;
            CargoType = cargoType;
            ServiceType = serviceType;
            CashOnDelivery = cashOnDelivery;
            Warehouse = warehouse;
            CargoInvoice = cargoInvoice;
            ChangedAt = changedAt;
        }
    }
}
