using Dapper;
using LegacySql.Data;
using LegacySql.Domain.PartnerProductGroups;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.PartnerProductGroups
{
    public class PartnerProductGroupsRepository : ILegacyPartnerProductGroupsRepository
    {
        private readonly AppDbContext _mapDb;
        private readonly LegacyDbConnection _sqlConnection;

        public PartnerProductGroupsRepository(AppDbContext mapDb, LegacyDbConnection sqlConnection)
        {
            _mapDb = mapDb;
            _sqlConnection = sqlConnection;
        }

        private async Task<IEnumerable<PartnerProductGroupData>> GetPartnerProductGroupDataFromDb()
        {
            var procedure = "dbo.E21_model_get_segmentation_product_group";

            var partnerProductGroupData = await _sqlConnection.Connection.QueryAsync<PartnerProductGroupData>(
                sql: procedure,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 300);

            return partnerProductGroupData;
        }

        public async IAsyncEnumerable<PartnerProductGroup> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var partnerProductGroupData = await GetPartnerProductGroupDataFromDb();
            
            var maps = await Maps.Create(partnerProductGroupData, _mapDb, cancellationToken);
            var mapper = new PartnerProductGroupMapper(maps.PartnerProductGroupMap);
            var partnerProductGroups = partnerProductGroupData.Select(e => mapper.Map(e));

            foreach(var partnerProductGroup in partnerProductGroups)
            {
                yield return partnerProductGroup;
            }            
        }

        private class Maps
        {
            public IDictionary<int, Guid?> PartnerProductGroupMap { get; set; }

            public static async Task<Maps> Create(IEnumerable<PartnerProductGroupData> data, AppDbContext mapDb, CancellationToken cancellationToken)
            {
                var uniqPartnerProductGroupIds = new List<int>();

                foreach(var partnerProductGroup in data)
                {
                    uniqPartnerProductGroupIds.Add(partnerProductGroup.SegmentationProductGroupsId);
                }

                uniqPartnerProductGroupIds = uniqPartnerProductGroupIds.Distinct().ToList();

                var partnerProductGroupMap = await mapDb.PartnerProductGroupMaps.AsNoTracking()
                    .Where(cm => uniqPartnerProductGroupIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(cm => cm.LegacyId, cm => cm.ErpGuid, cancellationToken);

                return new Maps
                {
                    PartnerProductGroupMap = partnerProductGroupMap
                };
            }
        }
    }
}
