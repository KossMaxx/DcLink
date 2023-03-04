using System;

namespace MessageBus.Firms.Export
{
    public class FirmDto
    {
        public int Code { get; set; }
        public string Title { get; set; }
        public string TaxCode { get; set; }
        public bool NotVat { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? MasterClientId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string LegalAddress { get; set; }
        public string PayerCode { get; set; }
        public string CertificateNumber { get; set; }
        public string Account { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public bool IsNotResident { get; set; }
    }
}
