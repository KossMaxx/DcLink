using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegacySql.Domain.Deliveries.Inner;
using LegacySql.Domain.Departments;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ClientOrders
{
    public class ClientOrder : Mapped
    {
        public IdMap Id { get; }
        public DateTime? Date { get; }
        public IdMap ClientId { get; }
        public string Comments { get; }
        public PaymentTypes PaymentType { get; }
        public string MarketplaceNumber { get; }
        public ClientOrderSource Source { get; }
        public IEnumerable<ClientOrderItem> Items { get; }
        public Delivery Delivery { get; }
        public bool IsExecuted { get; }
        public DateTime? ChangedAt { get; }
        public bool IsPaid { get; }
        public IdMap WarehouseId { get; }
        public IdMap ManagerId { get; }
        public int Quantity { get; }
        public double Amount { get; }
        public string RecipientOKPO { get; }
        public DateTime? PaymentDate { get; }
        public int? BillNumber { get; }
        public int? FirmSqlId { get; }

        public ClientOrder(IdMap id,
            DateTime? date,
            IdMap clientId,
            string comments,
            bool hasMap,
            PaymentTypes paymentType,
            string marketplaceNumber,
            ClientOrderSource source,
            Delivery delivery,
            bool isExecuted,
            DateTime? changedAt,
            bool isPaid,
            IdMap warehouseId,
            IdMap managerId, 
            int quantity, 
            double amount, 
            string recipientOKPO, 
            DateTime? paymentDate, 
            int? billNumber, 
            int? firmSqlId,
            IEnumerable<ClientOrderItem> items = null) : base(hasMap)
        {
            Id = id;
            Date = date;
            ClientId = clientId;
            Comments = comments;
            PaymentType = paymentType;
            MarketplaceNumber = marketplaceNumber;
            Source = source;
            Delivery = delivery;
            IsExecuted = isExecuted;
            ChangedAt = changedAt;
            IsPaid = isPaid;
            WarehouseId = warehouseId;
            ManagerId = managerId;
            Quantity = quantity;
            Amount = amount;
            RecipientOKPO = recipientOKPO;
            PaymentDate = paymentDate;
            BillNumber = billNumber;
            FirmSqlId = firmSqlId;
            Items = items ?? new List<ClientOrderItem>();
        }

        public MappingInfo IsMappingsFull()
        {
            var isClientMappingFull = ClientId?.ExternalId != null;
            var isWarehouseMappingFull = WarehouseId == null || WarehouseId?.ExternalId != null;
            var isManagerMappingFull = ManagerId == null || ManagerId?.ExternalId != null;
            var itemsWithoutMappings = Items.Where(i => i.NomenclatureId?.ExternalId == null).ToList();

            var isMappingFull = isClientMappingFull
                                && itemsWithoutMappings.Count == 0
                                && isWarehouseMappingFull
                                && isManagerMappingFull;

            if (isMappingFull)
            {
                return new MappingInfo { IsMappingFull = true, Why = "" };
            }

            var why = new StringBuilder();

            if (!isClientMappingFull)
            {
                why.AppendLine($"Поле: ClientId., Id: {ClientId?.InnerId}");
            }
            if (!isWarehouseMappingFull)
            {
                why.AppendLine($"Поле: WarehouseId., Id: {WarehouseId?.InnerId}");
            }
            if (!isManagerMappingFull)
            {
                why.AppendLine($"Поле: ManagerId., Id: {ManagerId?.InnerId}");
            }
            if (itemsWithoutMappings.Count > 0)
            {
                foreach (var item in itemsWithoutMappings)
                {
                    why.Append($"Поле: NomenclatureId., Id: {item.NomenclatureId?.InnerId}");
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
