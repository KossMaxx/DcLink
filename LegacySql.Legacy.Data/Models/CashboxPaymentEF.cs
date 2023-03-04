using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Касса")]
    public class CashboxPaymentEF
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CashboxId { get; set; }
        public int ClientId { get; set; }
        public decimal AmountUSD { get; set; }
        public decimal AmountUAH { get; set; }
        public decimal AmountEuro { get; set; }
        public decimal Rate { get; set; }
        public decimal RateEuro { get; set; }
        public string Description { get; set; }
        public string CreateUsername { get; set; }
        public DateTime? CreateDate { get; set; }
        public string ChangeUsername { get; set; }
        public DateTime? ChangeDate { get; set; }
    }
}
