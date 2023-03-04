using System;

namespace MessageBus.Cashboxes.Export
{
    public class CashboxApplicationPaymentDto
    {
        public int SqlId { get; set; }
        public DateTime Date { get; set; }
        public Guid WriteOffCliectId { get; set; }
        public Guid ReceiveClientId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
