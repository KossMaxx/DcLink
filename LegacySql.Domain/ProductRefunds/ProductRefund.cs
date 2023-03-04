using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductRefunds
{
    public class ProductRefund : Mapped
    {
        public IdMap Id { get; }
        public DateTime Date { get; }
        public DateTime? ChangedAt { get; }
        public IdMap ClientId { get; }
        public IdMap ClientOrderId { get; }
        public IEnumerable<ProductRefundItem> Items { get; }

        public ProductRefund(bool hasMap, IdMap id, DateTime date, DateTime? changedAt, IdMap clientId, IEnumerable<ProductRefundItem> items, IdMap clientOrderId) : base(hasMap)
        {
            Id = id;
            Date = date;
            ClientId = clientId;
            ClientOrderId = clientOrderId;
            ChangedAt = changedAt;
            Items = items ?? new List<ProductRefundItem>();
        }

        public MappingInfo IsMappingsFull()
        {
            var why = new StringBuilder();
            var isClientMapFull = ClientId?.ExternalId != null;
            if (!isClientMapFull)
            {
                why.Append($"Поле: ClientId, Id: {ClientId?.InnerId}\n");
            }

            var isItemsMapsFull = Items.All(i => i.ProductId?.ExternalId != null);
            if (!isItemsMapsFull)
            {
                var notFullMapsInfo = Items.Where(i => i.ProductId?.ExternalId == null).Select(i => $"Поле: Item.ProductId., Id: {i.ProductId?.InnerId}\n");
                foreach (var info in notFullMapsInfo)
                {
                    why.Append(info);
                }
            }

            return new MappingInfo
            {
                IsMappingFull = isClientMapFull && isItemsMapsFull,
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