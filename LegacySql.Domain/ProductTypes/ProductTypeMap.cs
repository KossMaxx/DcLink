using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductTypes
{
    public class ProductTypeMap : ExternalMap
    {
        public string Title { get; }

        public ProductTypeMap(Guid mapId, int legacyId, string title) : base(mapId, legacyId)
        {
            Title = title;
        }

        public ProductTypeMap(Guid mapId, int legacyId, Guid? externalMapId = null, Guid? id = null) : base(mapId, legacyId, externalMapId, id)
        {
        }
    }
}
