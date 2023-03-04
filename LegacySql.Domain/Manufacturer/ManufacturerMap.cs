using System;

namespace LegacySql.Domain.Manufacturer
{
    public class ManufacturerMap
    {
        public Guid Id { get; }
        public Guid MapId { get; }
        public Guid? ExternalMapId { get; private set; }
        public int LegacyId { get; }
        public string LegacyTitle { get; }

        public ManufacturerMap(Guid mapId, int legacyId, string legacyTitle)
        {
            MapId = mapId;
            LegacyId = legacyId;
            LegacyTitle = legacyTitle;
        }

        public ManufacturerMap(Guid mapId, int legacyId, string legacyTitle, Guid? externalMapId = null, Guid? id = null) : this(mapId, legacyId, legacyTitle)
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
