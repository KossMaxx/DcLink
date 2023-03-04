using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Firms")]
    public class FirmEF
    {
        public int Id { get; set; }//Код
        [Required]
        public string TaxCode { get; set; }//Окпо
        [Required]
        public string Title { get; set; }//Название
        public string LegalAddress { get; set; }//Адрес
        public string Address { get; set; }//AddressF
        public string Phone { get; set; }//Телефон
        public string Account { get; set; }//Рс
        public string BankCode { get; set; }//Мфо
        public string BankName { get; set; }//Банк
        public int? ClientId { get; set; }//klientID
        public virtual ClientEF Client { get; set; }
        public DateTime? LastChangeDate { get; set; }//DataLastChange
        public bool? NotVat { get; set; }//НеНДС
        public string PayerCode { get; set; }//Код_плат
        public string CertificateNumber { get; set; }//Номер_свид
        public bool IsNotResident { get; set; }//is_not_resident

    }
}
