using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegacySql.Domain.Firms;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Clients
{
    public class Client : Mapped
    {
        public IdMap Id { get; set; }
        public string Title { get; set; }
        public bool OnlySuperReports { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsCompetitor { get; set; }
        public string Email { get; set; }
        public int BalanceCurrencyId { get; set; }
        public IdMap MainManagerId { get; set; }
        public IdMap ResponsibleManagerId { get; set; }
        public short? CreditDays { get; set; }
        public byte PriceValidDays { get; set; }
        public IdMap MarketSegmentId { get; set; }
        public IdMap MarketSegmentationTurnoverId { get; set; }
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
        public DateTime? ChangedAt { get; set; }
        public IdMap DepartmentId { get; set; }
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
        public IEnumerable<Firm> Firms { get; set; }
        public IEnumerable<Client> Nested { get; set; }
        public IEnumerable<ClientDeliveryAddress> DeliveryAddresses { get; set; }
        public IEnumerable<ClientWarehousePriority> WarehousePriorities { get; set; }
        public IEnumerable<ClientWarehouseAccess> WarehouseAccesses { get; set; }
        public IEnumerable<ClientProductGroup> ClientProductGroups { get; set; }
        public IEnumerable<ClientActivityType> ClientActivityTypes { get; set; }

        public Client(bool hasMap) : base(hasMap)
        {

        }

        public MappingInfo IsMappingsFull()
        {
            var isMappingsFull = true;
            var why = new StringBuilder();

            if (MarketSegmentId != null && !MarketSegmentId.ExternalId.HasValue)
            {
                why.AppendLine($"Поле: MarketSegmentId. Id: {MarketSegmentId?.InnerId}");
                isMappingsFull = false;
            }

            if (MarketSegmentationTurnoverId != null && !MarketSegmentationTurnoverId.ExternalId.HasValue)
            {
                why.AppendLine($"Поле: MarketSegmentationTurnoverId. Id: {MarketSegmentationTurnoverId?.InnerId}");
                isMappingsFull = false;
            }

            if (DepartmentId != null && !DepartmentId.ExternalId.HasValue)
            {
                why.AppendLine($"Поле: DepartmentId. Id: {DepartmentId?.InnerId}");
                isMappingsFull = false;
            }

            if(ClientProductGroups != null && ClientProductGroups.Any())
            {
                foreach(var productGroup in ClientProductGroups)
                {
                    if(productGroup.Id.ExternalId == null)
                    {
                        why.AppendLine($"Поле: ClientProductGroups. Id: {productGroup.Id?.InnerId}");
                        isMappingsFull = false;
                    }
                }
            }

            if (ClientActivityTypes != null && ClientActivityTypes.Any())
            {
                foreach (var type in ClientActivityTypes)
                {
                    if (type.Id.ExternalId == null)
                    {
                        why.AppendLine($"Поле: ClientActivityTypes. Id: {type.Id?.InnerId}");
                        isMappingsFull = false;
                    }
                }
            }

            return new MappingInfo
            {
                IsMappingFull = isMappingsFull,
                Why = why.ToString()
            };
        }

        public bool IsNew()
        {
            return !HasMap;
        }

        public bool IsChanged()
        {
            return Id?.ExternalId != null;
        }
    }
}