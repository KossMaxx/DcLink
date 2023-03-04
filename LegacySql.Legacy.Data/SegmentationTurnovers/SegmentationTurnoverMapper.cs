using LegacySql.Domain.SegmentationTurnovers;
using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;

namespace LegacySql.Legacy.Data.SegmentationTurnovers
{
    internal class SegmentationTurnoverMapper
    {
        public IDictionary<int, Guid?> _segmentationTurnoverMap;

        public SegmentationTurnoverMapper(IDictionary<int, Guid?> segmentationTurnoverMap)
        {
            _segmentationTurnoverMap = segmentationTurnoverMap;
        }

        public SegmentationTurnover Map(SegmentationTurnoverData master)
        {
            var hasMap = _segmentationTurnoverMap.ContainsKey(master.SegmentationTurnoverId);
            return new SegmentationTurnover(
                    new IdMap(master.SegmentationTurnoverId,
                    hasMap ? _segmentationTurnoverMap[master.SegmentationTurnoverId] : null),
                    master.SegmentationTurnoverTitle,
                    hasMap
                );
        }
    }
}
