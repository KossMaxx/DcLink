using LegacySql.Domain.ActivityTypes;
using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;

namespace LegacySql.Legacy.Data.ActivityTypes
{
    internal class ActivityTypeMapper
    {
        public IDictionary<int, Guid?> _activityMap;

        public ActivityTypeMapper(IDictionary<int, Guid?> activityMap)
        {
            _activityMap = activityMap;
        }

        public ActivityType Map(ActivityTypeData master)
        {
            var hasMap = _activityMap.ContainsKey(master.SegmentationActivityTypeId);
            return new ActivityType(
                    new IdMap(master.SegmentationActivityTypeId,
                    hasMap ? _activityMap[master.SegmentationActivityTypeId] : null),
                    master.SegmentationActivityTypeTitle,
                    hasMap
                );
        }
    }
}
