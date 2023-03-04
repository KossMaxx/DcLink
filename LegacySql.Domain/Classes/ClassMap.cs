using System;

namespace LegacySql.Domain.Classes
{
    public class ClassMap
    {
        public Guid Id { get; }
        public Guid MapId { get; }
        public Guid? ExternalMapId { get; private set; }
        public string LegacyTitle { get; }

        public ClassMap(Guid mapId, string legacyTitle)
        {
            MapId = mapId;
            LegacyTitle = legacyTitle;
        }

        public ClassMap(Guid mapId, string legacyTitle, Guid? externalMapId = null, Guid? id = null) : this(mapId, legacyTitle)
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
