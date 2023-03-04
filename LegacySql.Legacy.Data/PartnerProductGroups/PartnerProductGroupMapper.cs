using LegacySql.Domain.PartnerProductGroups;
using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;

namespace LegacySql.Legacy.Data.PartnerProductGroups
{
    internal class PartnerProductGroupMapper
    {
        public IDictionary<int, Guid?> _partnerProductGroupMap;

        public PartnerProductGroupMapper(IDictionary<int, Guid?> partnerProductGroupMap)
        {
            _partnerProductGroupMap = partnerProductGroupMap;
        }

        public PartnerProductGroup Map(PartnerProductGroupData master)
        {
            var hasMap = _partnerProductGroupMap.ContainsKey(master.SegmentationProductGroupsId);
            return new PartnerProductGroup(
                    new IdMap(master.SegmentationProductGroupsId,
                    hasMap ? _partnerProductGroupMap[master.SegmentationProductGroupsId] : null),
                    master.SegmentationProductGroupsTitle,
                    hasMap
                );
        }
    }
}
