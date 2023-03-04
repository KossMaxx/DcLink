using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Клиенты")]
    public class ClientEF
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }//Название
        public bool? OnlySuperReports { get; set; }//H
        public bool IsSupplier { get; set; }//Поставщик
        public bool IsCustomer { get; set; }//Покупатель
        public bool IsCompetitor { get; set; }//Konkurent
        public string Email { get; set; }//email
        public DateTime? ChangedAt { get; set; }//modified_at
        public int MasterId { get; set; }//masterID
        public virtual ClientEF Master { get; set; }
        public byte BalanceCurrencyId { get; set; }//ВалютаБаланса
        public int? Department { get; set; }//department
        public bool IsTechnicalAccount { get; set; }//price_log
        public short? CreditDays { get; set; }//kredit
        public byte PriceValidDays { get; set; }//PriceValidDays
        public int? MainManagerId { get; set; }//manager1
        public int? ResponsibleManagerId { get; set; }//manager2
        public int? MarketSegmentId { get; set; }//segmentation
        public decimal? Credit { get; set; }//кредит
        public short? SurchargePercents { get; set; }//penyaV
        public short? BonusPercents { get; set; }//bonusV
        public bool DelayOk { get; set; }//delayOk
        public string DeliveryTel { get; set; }//dostavkaTel
        public string City { get; set; } //Город
        public bool? SegmentAccessories { get; set; }//segment_accessories
        public bool? SegmentActiveNet { get; set; }//segment_active_net
        public bool? SegmentAv { get; set; }//segment_AV
        public bool? SegmentComponentsPc { get; set; }//segment_componentsPC
        public bool? SegmentExpendables { get; set; }//segment_expendables
        public bool? SegmentKbt { get; set; }//segment_KBT
        public bool? SegmentMbt { get; set; }//segment_MBT
        public bool? SegmentMobile { get; set; }//segment_mobile
        public bool? SegmentNotebooks { get; set; }//segment_notebooks
        public bool? SegmentPassiveNet { get; set; }//segment_passive_net
        public bool? SegmentPeriphery { get; set; }//segment_periphery
        public bool? SegmentPrint { get; set; }//segment_print
        public bool? SegmentReadyPc { get; set; }//segment_readyPC
        public bool Consig { get; set; }//consig
        public bool IsPcAssembler { get; set; }//is_PC_assembler
        public bool SegmentNetSpecifility { get; set; }//segment_net_specifility

        public string Website { get; set; }//website

        public string ContactPerson { get; set; }//ОбращатьсяК
        public string ContactPersonPhone { get; set; }//НомерТелефона
        public string Address { get; set; }//Адрес
        public string MobilePhone { get; set; }//cell_ID

        public byte DefaultPriceColumn { get; set; }//колонка

        public virtual ICollection<FirmEF> Firms { get; set; }
        public virtual ICollection<ClientEF> Nested { get; set; }
        public virtual ICollection<ClientDeliveryAddressEF> DeliveryAddresses { get; set; }
        public virtual ICollection<ClientWarehousePriorityEF> WarehousePriorities { get; set; }
        public virtual ICollection<ClientWarehouseAccessEF> WarehouseAccesses { get; set; }
    }
}
