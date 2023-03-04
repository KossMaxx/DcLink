using System;

namespace LegacySql.Domain.Shared
{
    public class ExternalMap
    {
        public Guid Id { get; }
        public Guid MapId { get; }
        public Guid? ExternalMapId { get; private set; }
        public int LegacyId { get; }

        public ExternalMap(Guid mapId, int legacyId)
        {
            MapId = mapId;
            LegacyId = legacyId;
        }

        public ExternalMap(Guid mapId, int legacyId, Guid? externalMapId = null, Guid? id = null) : this(mapId, legacyId)
        {
            ExternalMapId = externalMapId;
            Id = id ?? Guid.Empty;
        }

        public void MapToExternalId(Guid externalMapId)
        {
            ExternalMapId = externalMapId;
        }
    }
}
