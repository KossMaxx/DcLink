using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Legacy.Data.Clients
{
    internal class FirmData
    {
        public int Firms_Id { get; set; }
        public string Firms_TaxCode { get; set; }
        public string Firms_Title { get; set; }
        public string Firms_LegalAddress { get; set; }
        public string Firms_Address { get; set; }
        public string Firms_Phone { get; set; }
        public string Firms_Account { get; set; }
        public string Firms_BankCode { get; set; }
        public string Firms_BankName { get; set; }
        public bool Firms_IsNotResident { get; set; }
        public int Firms_ClientId { get; set; }
        public DateTime? Firms_LastChangeDate { get; set; }
        public string Firms_PayerCode { get; set; }
        public string Firms_CertificateNumber { get; set; }
        public bool? NotVat { get; set; }
    }
}
