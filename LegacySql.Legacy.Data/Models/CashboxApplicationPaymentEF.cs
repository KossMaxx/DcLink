using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("TBL_Fin_MoneyTransferApplications")]
    public class CashboxApplicationPaymentEF
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int WriteOffCliectId { get; set; }
        public int ReceiveClientId { get; set; }
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime? ChangeDate { get; set; }
        public bool HeldIn { get; set; }
    }
}
