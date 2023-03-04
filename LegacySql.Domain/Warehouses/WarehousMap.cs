using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Warehouses
{
    public class WarehouseMap : ExternalMap
    {
        public WarehouseMap(Guid mapId, int legacyId) : base(mapId, legacyId)
        {
        }

        public WarehouseMap(Guid mapId, int legacyId, Guid? externalMapId = null, Guid? id = null)
            : base(mapId, legacyId, externalMapId, id)
        {
        }
    }
}
