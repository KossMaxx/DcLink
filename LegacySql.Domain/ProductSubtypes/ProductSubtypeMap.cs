using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductSubtypes
{
    public class ProductSubtypeMap : ExternalMap

    {
        public ProductSubtypeMap(Guid mapId, int legacyId, string title) : base(mapId, legacyId)
        {
            Title = title;
        }

        public ProductSubtypeMap(Guid mapId, int legacyId, string title, Guid? externalMapId = null, Guid? id = null) : base(mapId, legacyId, externalMapId, id)
        {
            Title = title;
        }

        public string Title { get; }
    }
}
