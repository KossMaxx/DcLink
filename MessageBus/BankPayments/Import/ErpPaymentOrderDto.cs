using System;

namespace MessageBus.BankPayments.Import
{
    public class ErpPaymentOrderDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public short OrderNumber { get; set; }
        public int InnerDocNumber { get; set; }
        public decimal Sum { get; set; }
        public string Purpose { get; set; }
        public string Okpo { get; set; }
        public string Company { get; set; }
        public string Account { get; set; }
        public string Mfo { get; set; }
        public Guid ClientId { get; set; }
        public decimal Rate { get; set; }
        public bool IsConfirmed { get; set; }
        public int? PurchaseId { get; set; }
        public string Username { get; set; }
        public int CurrencyId { get; set; }
    }
}
