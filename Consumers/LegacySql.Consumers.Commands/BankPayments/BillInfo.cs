using System;

namespace LegacySql.Consumers.Commands.BankPayments
{
    internal class BillInfo
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime? Date { get; set; }
        public int? CompanyId { get; set; }
        public int? FirmId { get; set; }
    }
}
