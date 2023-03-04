using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("dostavka")]
    public class DeliveryEF
    {
        public int Id { get; set; } //id
        public string RecipientName { get; set; } //poluchatel
        public string RecipientPhone { get; set; } //tel
        public string RecipientAddress { get; set; } //kuda
        public Guid? RecipientCityId { get; set; } //CityRecipient
        public short? TypeId { get; set; } //gorod
        public float? Weight { get; set; } //Weight
        public float? Volume { get; set; } //Vol
        public decimal? DeclaredPrice { get; set; } //DeclaredPrice
        public string PayerType { get; set; } //PayerType
        public string PaymentMethod { get; set; } //PaymentMethod
        public string CargoType { get; set; } //CargoType
        public string ServiceType { get; set; } //ServiceType
        public bool CashOnDelivery { get; set; } //CashOnDelivery
        public string RecipientEmail { get; set; } //email
        public string CargoInvoice { get; set; } //CargoInvoice
        public DateTime? ChangedAt { get; set; } //modified_at
    }
}