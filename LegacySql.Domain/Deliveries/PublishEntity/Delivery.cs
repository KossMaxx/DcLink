using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegacySql.Domain.Deliveries.PublishEntity
{
    public class Delivery : Mapped
    {
        public IdMap Id { get; }
        public DateTime? CreateDate { get; }
        public string DeparturePlanDate { get; }
        public string CreatorUsername { get; }
        public string TaskDescription { get; }
        public string Address { get; }
        public string CarrierTypeName { get; }
        public string CargoInvoice { get; }
        public int StatusId { get; }
        public DeliveryCategory Category { get; }
        public IdMap ReceivedEmployeeId { get; }
        public bool? IsCompleted { get; }
        public DateTime? PerformedDate { get; }
        public bool IsFinished { get; }
        public DateTime? ChangedAt { get; }
        public IEnumerable<string> Stickers { get; }
        public IEnumerable<DeliveryWarehouse> Warehouses { get; }


        public Delivery(
            bool hasMap,
            IdMap id,
            DateTime? createDate,
            string departurePlanDate,
            string creatorUsername,
            string taskDescription,
            string address,
            string carrierTypeName,
            string cargoInvoice,
            int statusId,
            DeliveryCategory category,
            IdMap receivedEmployeeId,
            bool? isCompleted,
            DateTime? performedDate,
            bool isFinished,
            DateTime? changedAt,
            IEnumerable<string> stickers,
            IEnumerable<DeliveryWarehouse> warehouses) : base(hasMap)
        {
            Id = id;
            CreateDate = createDate;
            DeparturePlanDate = departurePlanDate;
            CreatorUsername = creatorUsername;
            TaskDescription = taskDescription;
            Address = address;
            CarrierTypeName = carrierTypeName;
            CargoInvoice = cargoInvoice;
            StatusId = statusId;
            ReceivedEmployeeId = receivedEmployeeId;
            IsCompleted = isCompleted;
            PerformedDate = performedDate;
            IsFinished = isFinished;
            ChangedAt = changedAt;
            Stickers = stickers;
            Warehouses = warehouses;
            Category = category;
        }

        public MappingInfo IsMappingsFull()
        {
            var isReceivedEmployeeMappingFull = ReceivedEmployeeId?.ExternalId != null;
            var warehousesWithoutMappings = Warehouses.Where(i => i.ClientOrderId != null && i.ClientOrderId.ExternalId == null).ToList();

            var isMappingFull = isReceivedEmployeeMappingFull
                                && warehousesWithoutMappings.Count == 0;

            if (isMappingFull)
            {
                return new MappingInfo { IsMappingFull = true, Why = "" };
            }

            var why = new StringBuilder();

            if (!isReceivedEmployeeMappingFull)
            {
                why.AppendLine($"Поле: ReceivedEmployeeId, Id: {ReceivedEmployeeId?.InnerId}");
            }

            if (warehousesWithoutMappings.Count > 0)
            {
                foreach (var item in warehousesWithoutMappings)
                {
                    why.Append($"Поле: ClientOrderId, Id: {item.ClientOrderId?.InnerId}");
                }
            }

            return new MappingInfo
            {
                IsMappingFull = false,
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
