using System;
using System.Collections.Generic;

namespace MessageBus.Clients.Import
{
    public class ErpPartnerDto
    {
        public Guid Id { get; set; }
        public Guid? MasterId { get; set; }
        public int? SupplierCode { get; set; }
        public string Title { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsCompetitor { get; set; }
        public string Email { get; set; }
        public int BalanceCurrencyId { get; set; }
        public bool IsNew { get; set; }
        public Guid? MainManagerId { get; set; }
        public Guid? ResponsibleManagerId { get; set; }
        public short? CreditDays { get; set; }
        public byte PriceValidDays { get; set; }
        public Guid? MarketSegmentId { get; set; }
        public Guid? MarketSegmentationTurnoverId { get; set; }
        public decimal? Credit { get; set; }
        public short? SurchargePercents { get; set; }
        public short? BonusPercents { get; set; }
        public bool DelayOk { get; set; }
        public string DeliveryTel { get; set; }
        public string Website { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonPhone { get; set; }
        public string Address { get; set; }
        public string MobilePhone { get; set; }
        public byte DefaultPriceColumn { get; set; }
        public Guid? DepartmentId { get; set; }
        public string City { get; set; }
        public string ScContactPerson { get; set; }
        public string ScContactPhone { get; set; }
        public string ScContactEmail { get; set; }
        public string ScDeliveryAddress { get; set; }
        public string ScDeliveryRecipient { get; set; }
        public string ScDeliveryPhone { get; set; }
        public IEnumerable<ErpClientDeliveryAddressDto> DeliveryAddresses { get; set; }
        public IEnumerable<ErpClientWarehousePriorityDto> WarehousePriorities { get; set; }
        public IEnumerable<ErpClientWarehouseAccessDto> WarehouseAccesses { get; set; }
    }
}
