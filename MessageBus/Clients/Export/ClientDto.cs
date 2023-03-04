using System;
using System.Collections.Generic;

namespace MessageBus.Clients.Export
{
    public class ClientDto
    {
        public int SupplierCode { get; set; }
        public string Title { get; set; }
        public bool OnlySuperReports { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsCompetitor { get; set; }
        public string Email { get; set; }
        public int BalanceCurrencyId { get; set; }
        public Guid? MainManagerId { get; set; }
        public Guid? ResponsibleManagerId { get; set; }
        public short? CreditDays { get; set; }
        public byte PriceValidDays { get; set; }
        public IEnumerable<FirmDto> Firms { get; set; }
        public Guid? MarketSegmentId { get; set; }
        public Guid? MarketSegmentationTurnoverId { get; set; }
        public decimal? Credit { get; set; }
        public short? SurchargePercents { get; set; }
        public short? BonusPercents { get; set; }
        public bool DelayOk { get; set; }
        public string DeliveryTel { get; set; }
        public bool SegmentAccessories { get; set; }
        public bool SegmentActiveNet { get; set; }
        public bool SegmentAv { get; set; }
        public bool SegmentComponentsPc { get; set; }
        public bool SegmentExpendables { get; set; }
        public bool SegmentKbt { get; set; }
        public bool SegmentMbt { get; set; }
        public bool SegmentMobile { get; set; }
        public bool SegmentNotebooks { get; set; }
        public bool SegmentPassiveNet { get; set; }
        public bool SegmentPeriphery { get; set; }
        public bool SegmentPrint { get; set; }
        public bool SegmentReadyPc { get; set; }
        public bool Consig { get; set; }
        public bool IsPcAssembler { get; set; }
        public bool SegmentNetSpecifility { get; set; }
        public string Website { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhone { get; set; }
        public string Address { get; set; }
        public string MobilePhone { get; set; }
        public byte DefaultPriceColumn { get; set; }
        public Guid? DepartmentId { get; set; }
        public string City { get; set; }
        public int? RegionId { get; set; }
        public string RegionTitle { get; set; }
        public bool? Bonus { get; set; }
        public bool? Penya { get; set; }
        public string ScContactPerson { get; set; }
        public string ScContactPhone { get; set; }
        public string ScContactEmail { get; set; }
        public string ScDeliveryAddress { get; set; }
        public string ScDeliveryRecipient { get; set; }
        public string ScDeliveryPhone { get; set; }
        public IEnumerable<ClientDeliveryAddressDto> DeliveryAddresses { get; set; }
        public IEnumerable<ClientWarehousePriorityDto> WarehousePriorities { get; set; }
        public IEnumerable<ClientWarehouseAccessDto> WarehouseAccesses { get; set; }
        public IEnumerable<ClientProductGroupDto> ClientProductGroups { get; set; }
        public IEnumerable<ClientActivityTypeDto> ClientActivityTypes { get; set; }
    }
}
