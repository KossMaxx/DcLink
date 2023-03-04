using System;

namespace LegacySql.Legacy.Data.Clients
{
    internal class ClientData
    {
        public int Client_Id { get; set; }
        public int Client_MasterId { get; set; }
        public string Client_Title { get; set; }
        public bool? Client_OnlySuperReports { get; set; }
        public bool Client_IsSupplier { get; set; }
        public bool Client_IsCustomer { get; set; }
        public bool Client_IsCompetitor { get; set; }
        public string Client_Email { get; set; }
        public DateTime? Client_ChangedAt { get; set; }
        public byte Client_BalanceCurrencyId { get; set; }
        public int? Client_Department { get; set; }
        public bool Client_IsTechnicalAccount { get; set; }
        public short? Client_CreditDays { get; set; }
        public byte Client_PriceValidDays { get; set; }
        public int? Client_MainManagerId { get; set; }
        public int? Client_ResponsibleManagerId { get; set; }
        public int? Client_MarketSegmentId { get; set; }
        public int? Client_MarketSegmentationTurnover { get; set; }
        public decimal? Client_Credit { get; set; }
        public short? Client_SurchargePercents { get; set; }
        public short? Client_BonusPercents { get; set; }
        public bool Client_DelayOk { get; set; }
        public string Client_DeliveryTel { get; set; }
        public Guid? Client_City { get; set; }
        public bool? Client_SegmentAccessories { get; set; }
        public bool? Client_SegmentActiveNet { get; set; }
        public bool? Client_SegmentAv { get; set; }
        public bool? Client_SegmentComponentsPc { get; set; }
        public bool? Client_SegmentExpendables { get; set; }
        public bool? Client_SegmentKbt { get; set; }
        public bool? Client_SegmentMbt { get; set; }
        public bool? Client_SegmentMobile { get; set; }
        public bool? Client_SegmentNotebooks { get; set; }
        public bool? Client_SegmentPassiveNet { get; set; }
        public bool? Client_SegmentPeriphery { get; set; }
        public bool? Client_SegmentPrint { get; set; }
        public bool? Client_SegmentReadyPc { get; set; }
        public bool Client_Consig { get; set; }
        public bool Client_IsPcAssembler { get; set; }
        public bool Client_SegmentNetSpecifility { get; set; }
        public string Client_Website { get; set; }
        public string Client_ContactPerson { get; set; }
        public string Client_ContactPersonPhone { get; set; }
        public string Client_Address { get; set; }
        public string Client_MobilePhone { get; set; }
        public byte Client_DefaultPriceColumn { get; set; }
        public int? Client_RegionId { get; set; }
        public string Client_RegionTitle { get; set; }
        public bool? Client_Bonus { get; set; }
        public bool? Client_Penya { get; set; }
        public string Client_Sc_ContactPerson { get; set; }
        public string Client_Sc_ContactPhone { get; set; }
        public string Client_Sc_ContactEmail { get; set; }
        public string Client_Sc_DeliveryAddress { get; set; }
        public string Client_Sc_DeliveryRecipient { get; set; }
        public string Client_Sc_DeliveryPhone { get; set; }
        public FirmData Firm { get; set; }
        public DeliveryAddressData Address { get; set; }
        public WarehousePriorityData WarehousePriority { get; set; }
        public WarehouseAccessData WarehouseAccess { get; set; }
    }
}
