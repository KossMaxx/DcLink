using LegacySql.Domain.ProductMoving;
using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacySql.Legacy.Data.ProductMovings
{
    public class ProductMovingMapper
    {
        public IDictionary<int, Guid?> _productTransferMap;
        public IDictionary<int, Guid?> _productMap;
        public IDictionary<int, Guid?> _warehouseMap;
        public IDictionary<int, Guid?> _creatorMap;

        public ProductMovingMapper(
            IDictionary<int, Guid?> productTransferMap,
            IDictionary<int, Guid?> productMap,
            IDictionary<int, Guid?> warehouseMap,
            IDictionary<int, Guid?> creatorMap)
        {
            _productTransferMap = productTransferMap;
            _productMap = productMap;
            _warehouseMap = warehouseMap;
            _creatorMap = creatorMap;
        }

        public ProductMoving Map(ProductMovingData master, IEnumerable<ProductMovingData> items)
        {
            var hasMap = _productTransferMap.ContainsKey(master.Id);

            var transfer =  new ProductMoving(
                hasMap: hasMap,
                id: new IdMap(master.Id, hasMap ? _productTransferMap[master.Id] : null),
                date: master.Date,
                creatorUsername: master.CreatorUsername,
                creatorId: master.CreatorId.HasValue && _creatorMap.ContainsKey(master.CreatorId.Value) 
                ? new IdMap(master.CreatorId.Value, _creatorMap[master.CreatorId.Value])
                : master.CreatorId.HasValue && !_creatorMap.ContainsKey(master.CreatorId.Value) 
                ? new IdMap(master.CreatorId.Value, null)
                : null,
                outWarehouseId: _warehouseMap.ContainsKey(master.OutWarehouseId) 
                ? new IdMap(master.OutWarehouseId, _warehouseMap[master.OutWarehouseId])
                : new IdMap(master.OutWarehouseId, null),
                inWarehouseId: _warehouseMap.ContainsKey(master.InWarehouseId)
                ? new IdMap(master.InWarehouseId, _warehouseMap[master.InWarehouseId])
                : new IdMap(master.InWarehouseId, null),
                description: master.Description,
                okpo: master.Okpo,
                shippedDate: master.ShippedDate,
                forShipmentDate: master.ForShipmentDate,
                changedAt: master.ChangedAt,
                isAccepted: master.IsAccepted,
                isForShipment: master.IsForShipment,
                isShipped: master.IsShipped);

            transfer.Items = items.Select(e=> new ProductMovingItem(
                productId: _productMap.ContainsKey(e.Item.ProductId) 
                ? new IdMap(e.Item.ProductId, _productMap[e.Item.ProductId])
                : new IdMap(e.Item.ProductId, null),
                amount: e.Item.Amount ?? 0,
                price: e.Item.Price ?? 0
                ));

            return transfer;
        }
    }
}
