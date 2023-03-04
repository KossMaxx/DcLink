using System;

namespace MessageBus.Cashboxes.Import
{
    public class ErpCashboxPaymentDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid CashboxId { get; set; }
        public decimal AmountUsd { get; set; }
        public decimal AmountUah { get; set; }
        public decimal AmountEuro { get; set; }
        public decimal Rate { get; set; }
        public decimal RateEuro { get; set; }
        public string Description { get; set; }
        public Guid ClientId { get; set; }
        public bool IsTransit { get; set; }
        public string CreatorUsername { get; set; }
        public string UpdatorUsername { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
