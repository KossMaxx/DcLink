using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ProductTypeCategoryGroups
{
    public class ProductTypeCategoryGroupMap : ExternalMap
    {
        public string Name { get; }

        public ProductTypeCategoryGroupMap(Guid mapId, int legacyId, string name) : base(mapId, legacyId)
        {
            Name = name;
        }

        public ProductTypeCategoryGroupMap(Guid mapId, int legacyId, Guid? externalMapId = null, Guid? id = null) : base(mapId, legacyId, externalMapId, id)
        {
        }
    }
}
