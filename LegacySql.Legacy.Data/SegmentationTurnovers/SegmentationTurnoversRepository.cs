using Dapper;
using LegacySql.Data;
using LegacySql.Domain.SegmentationTurnovers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.SegmentationTurnovers
{
    public class SegmentationTurnoversRepository : ILegacySegmentationTurnoversRepository
    {
        private readonly AppDbContext _mapDb;
        private readonly LegacyDbConnection _sqlConnection;

        public SegmentationTurnoversRepository(AppDbContext mapDb, LegacyDbConnection sqlConnection)
        {
            _mapDb = mapDb;
            _sqlConnection = sqlConnection;
        }

        private async Task<IEnumerable<SegmentationTurnoverData>> GetSegmentationTurnoversFromDb()
        {
            var procedure = "dbo.E21_model_get_segmentation_turnover";

            var segmentationTurnoverData = await _sqlConnection.Connection.QueryAsync<SegmentationTurnoverData>(
                sql: procedure,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 300);

            return segmentationTurnoverData;
        }

        public async IAsyncEnumerable<SegmentationTurnover> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var segmentationTurnoversData = await GetSegmentationTurnoversFromDb();

            var maps = await Maps.Create(segmentationTurnoversData, _mapDb, cancellationToken);
            var mapper = new SegmentationTurnoverMapper(maps.SegmentationTurnoverMap);
            var segmentationTurnovers = segmentationTurnoversData.Select(e => mapper.Map(e));

            foreach (var segmentationTurnover in segmentationTurnovers)
            {
                yield return segmentationTurnover;
            }
        }
        
        private class Maps
        {
            public IDictionary<int, Guid?> SegmentationTurnoverMap { get; set; }

            public static async Task<Maps> Create(IEnumerable<SegmentationTurnoverData> data, AppDbContext mapDb, CancellationToken cancellationToken)
            {
                var uniqSegmentationTurnoverIds = new List<int>();

                foreach (var segmentationTurnover in data)
                {
                    uniqSegmentationTurnoverIds.Add(segmentationTurnover.SegmentationTurnoverId);
                }

                uniqSegmentationTurnoverIds = uniqSegmentationTurnoverIds.Distinct().ToList();

                var segmentationTurnoverIdMap = await mapDb.SegmentationTurnoverMaps.AsNoTracking()
                    .Where(cm => uniqSegmentationTurnoverIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(cm => cm.LegacyId, cm => cm.ErpGuid, cancellationToken);

                return new Maps
                {
                    SegmentationTurnoverMap = segmentationTurnoverIdMap
                };
            }
        }
    }
}
