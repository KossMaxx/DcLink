using Dapper;
using LegacySql.Data;
using LegacySql.Domain.ActivityTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.ActivityTypes
{
    public class ActivityTypesRepository : ILegacyActivityTypesRepository
    {
        private readonly AppDbContext _mapDb;
        private readonly LegacyDbConnection _sqlConnection;

        public ActivityTypesRepository(AppDbContext mapDb, LegacyDbConnection sqlConnection)
        {
            _mapDb = mapDb;
            _sqlConnection = sqlConnection;
        }

        private async Task<IEnumerable<ActivityTypeData>> GetActivityTypesFromDb()
        {
            var procedure = "dbo.E21_model_get_segmentation_activity_type";

            var activityTypeData = await _sqlConnection.Connection.QueryAsync<ActivityTypeData>(
                sql: procedure,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 300);

            return activityTypeData;
        }

        public async IAsyncEnumerable<ActivityType> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var activityTypesData = await GetActivityTypesFromDb();

            var maps = await Maps.Create(activityTypesData, _mapDb, cancellationToken);
            var mapper = new ActivityTypeMapper(maps.ActivityTypeMap);
            var activityTypes = activityTypesData.Select(e => mapper.Map(e));

            foreach (var activityType in activityTypes)
            {
                yield return activityType;
            }
        }

        private class Maps
        {
            public IDictionary<int, Guid?> ActivityTypeMap { get; set; }

            public static async Task<Maps> Create(IEnumerable<ActivityTypeData> data, AppDbContext mapDb, CancellationToken cancellationToken)
            {
                var uniqActivityTypeIds = new List<int>();

                foreach (var activityType in data)
                {
                    uniqActivityTypeIds.Add(activityType.SegmentationActivityTypeId);
                }

                uniqActivityTypeIds = uniqActivityTypeIds.Distinct().ToList();

                var activityTypeMap = await mapDb.ActivityTypes.AsNoTracking()
                    .Where(cm => uniqActivityTypeIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(cm => cm.LegacyId, cm => cm.ErpGuid, cancellationToken);

                return new Maps
                {
                    ActivityTypeMap = activityTypeMap
                };
            }
        }
    }
}
