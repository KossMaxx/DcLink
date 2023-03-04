using LegacySql.Domain.Deliveries.Inner;
using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegacySql.Domain.Bills
{
    public class Bill : Mapped
    {
        public IdMap Id { get;  }
        public DateTime? Date { get; }
        public IdMap ClientId { get; }
        public string Comments { get; }
        public DateTime ChangedAt { get; }
        public string SellerOkpo { get; }
        public DateTime ValidToDate { get; }
        public string FirmOkpo { get; }
        public int? FirmSqlId { get; }
        public IdMap CreatorId { get; }
        public IdMap ManagerId { get; }
        public int? Number { get; }
        public bool? Issued { get; }
        public int Quantity { get; }
        public decimal Amount { get; }
        public IEnumerable<BillItem> Items { get; }
        public decimal TotalUah { get; }
        public decimal Total { get; }
        public Delivery Delivery { get; }

        public Bill(
            IdMap id,
            DateTime? date,
            IdMap clientId,
            string comments,
            DateTime changedAt,
            string sellerOkpo,
            DateTime validToDate,
            string firmOkpo,
            int? firmSqlId,
            IdMap creatorId,
            IdMap managerId,
            int? number,
            bool? issued,
            IEnumerable<BillItem> items,
            int quantity,
            decimal amount,
            decimal totalUah,
            decimal total,
            bool hasMap, 
            Delivery delivery) : base(hasMap)
        {
            Id = id;
            Date = date;
            ClientId = clientId;
            Comments = comments;
            ChangedAt = changedAt;
            SellerOkpo = sellerOkpo;
            ValidToDate = validToDate;
            FirmOkpo = firmOkpo;
            FirmSqlId = firmSqlId;
            CreatorId = creatorId;
            ManagerId = managerId;
            Number = number;
            Issued = issued;
            Items = items;
            Quantity = quantity;
            Amount = amount;
            TotalUah = totalUah;
            Total = total;
            Delivery = delivery;
        }

        public MappingInfo IsMappingsFull()
        {
            var isClientMappingFull = ClientId?.ExternalId != null;
            var isManagerMappingFull = ManagerId == null || ManagerId?.ExternalId != null;
            var isCreatorMappingFull = CreatorId == null || CreatorId?.ExternalId != null;
            var itemsWithoutMappings = Items.Where(i => i.NomenclatureId?.ExternalId == null).ToList();

            var isMappingFull = isClientMappingFull
                                && itemsWithoutMappings.Count == 0
                                && isManagerMappingFull;

            if (isMappingFull)
            {
                return new MappingInfo { IsMappingFull = true, Why = "" };
            }

            var why = new StringBuilder();

            if (!isClientMappingFull)
            {
                why.AppendLine($"Поле: ClientId, Id: {ClientId?.InnerId}");
            }
            if (!isManagerMappingFull)
            {
                why.AppendLine($"Поле: ManagerId, Id: {ManagerId?.InnerId}");
            }
            if (!isCreatorMappingFull)
            {
                why.AppendLine($"Поле: CreatorId, Id: {CreatorId?.InnerId}");
            }
            if (itemsWithoutMappings.Count > 0)
            {
                foreach (var item in itemsWithoutMappings)
                {
                    why.Append($"Поле: NomenclatureId, Id: {item.NomenclatureId?.InnerId}");
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
