using System;

namespace LegacySql.Api.Models
{
    public class CashboxApplicationPaymentCarryingInfo
    {
        public Guid OutPaymentId { get; set; }
        public Guid IncomePaymentId { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public bool HeldIn { get; set; }
    }
}
