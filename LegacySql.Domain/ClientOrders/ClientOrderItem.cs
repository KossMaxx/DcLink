using System;
using System.Collections.Generic;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ClientOrders
{
    public class ClientOrderItem
    {
        public IdMap NomenclatureId { get; }
        public decimal Quantity { get; }
        public decimal Price { get; }
        public decimal PriceUAH { get; }
        public short Warranty { get; }
        public DateTime? ChangedAt { get; }
        public IEnumerable<string> SerialNumbers { get; }

        public ClientOrderItem(IdMap nomenclatureId, decimal quantity, decimal? price, decimal? priceUah, short warranty, IEnumerable<string> serialNumbers, DateTime? changedAt)
        {
            NomenclatureId = nomenclatureId;
            Quantity = quantity;
            Warranty = warranty;
            SerialNumbers = serialNumbers;
            ChangedAt = changedAt;
            Price = price ?? 0;
            PriceUAH = priceUah ?? 0;
        }
    }
}
