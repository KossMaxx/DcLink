using System;

namespace MessageBus.Cashboxes.Export
{
    public class CashboxPaymentDto
    {
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public Guid CashboxId { get; set; }
        public Guid ClientId { get; set; }
        public decimal AmountUSD { get; set; }
        public decimal AmountUAH { get; set; }
        public decimal AmountEuro { get; set; }
        public decimal Rate { get; set; }
        public decimal RateEuro { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
    }
}
