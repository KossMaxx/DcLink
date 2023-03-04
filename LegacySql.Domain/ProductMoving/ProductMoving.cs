using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Domain.ProductMoving
{
    public class ProductMoving : Mapped
    {
        public IdMap Id { get; }
        public DateTime Date { get; }
        public DateTime? ShippedDate { get; }
        public DateTime? ForShipmentDate { get; }
        public string CreatorUsername { get; }
        public IdMap CreatorId { get; }
        public IdMap OutWarehouseId { get; }
        public IdMap InWarehouseId { get; }
        public string Description { get; }
        public string Okpo { get; }
        public DateTime ChangedAt { get; }
        public bool IsAccepted { get; }
        public bool IsForShipment { get; }
        public bool IsShipped { get; }
        public IEnumerable<ProductMovingItem> Items { get; set; }
        public ProductMoving(
            bool hasMap,
            IdMap id,
            DateTime date,
            string creatorUsername,
            IdMap creatorId,
            IdMap outWarehouseId,
            IdMap inWarehouseId,
            string description,
            string okpo,
            DateTime? shippedDate,
            DateTime? forShipmentDate,
            DateTime changedAt, 
            bool isAccepted, 
            bool isForShipment, 
            bool isShipped) : base(hasMap)
        {
            Id = id;
            Date = date;
            CreatorUsername = creatorUsername;
            CreatorId = creatorId;
            OutWarehouseId = outWarehouseId;
            InWarehouseId = inWarehouseId;
            Description = description;
            Okpo = okpo;
            ShippedDate = shippedDate;
            ForShipmentDate = forShipmentDate;
            ChangedAt = changedAt;
            IsAccepted = isAccepted;
            IsForShipment = isForShipment;
            IsShipped = isShipped;
        }

        public MappingInfo IsMappingsFull()
        {
            var isMappingsFull = true;
            var why = new StringBuilder();
            if (CreatorId != null && !CreatorId.ExternalId.HasValue)
            {
                why.Append($"Поле: CreatorId. Id: {CreatorId?.InnerId}\n");
                isMappingsFull = false;
            }
            if (!OutWarehouseId.ExternalId.HasValue)
            {
                why.Append($"Поле: OutWarehouseId. Id: {OutWarehouseId.InnerId}\n");
                isMappingsFull = false;
            }
            if (InWarehouseId != null && !InWarehouseId.ExternalId.HasValue)
            {
                why.Append($"Поле: InWarehouseId. Id: {InWarehouseId.InnerId}\n");
                isMappingsFull = false;
            }

            foreach (var item in Items)
            {
                if (!item.ProductId.ExternalId.HasValue)
                {
                    why.Append($"Поле: ProductId. Id: {item.ProductId.InnerId}\n");
                    isMappingsFull = false;
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
