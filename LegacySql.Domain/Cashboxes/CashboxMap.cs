using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Cashboxes
{
    public class CashboxMap : ExternalMap
    {
        public string Description { get; }

        public CashboxMap(Guid mapId, int legacyId, string description) : base(mapId, legacyId)
        {
            Description = description;
        }

        public CashboxMap(Guid mapId, int legacyId, string description, Guid? externalMapId = null, Guid? id = null) : base(mapId, legacyId, externalMapId, id)
        {
            Description = description;
        }
    }
}
