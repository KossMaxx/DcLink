using System;
using System.Collections.Generic;

namespace MessageBus.BankPayments.Import
{
    public class ErpBankPaymentDto
    {
        public Guid Id { get; set; }
        public decimal Rate { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public string BankNumber { get; set; }
        public string Okpo { get; set; }
        public string Company { get; set; }
        public bool IsConfirmed { get; set; }
        public int CurrencyId { get; set; }
        public bool IsLiqpay { get; set; }
        public IEnumerable<ErpBankPaymentClientOrderDto> ClientOrders { get; set; }
    }

    public class ErpBankPaymentClientOrderDto
    {
        public Guid ClientId { get; set; }
        public int? ClientOrderId { get; set; }
        public decimal Amount { get; set; }
        public bool IsBill { get; set; }
    }
}
