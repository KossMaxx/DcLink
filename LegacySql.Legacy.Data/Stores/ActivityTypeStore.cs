using Dapper;
using LegacySql.Legacy.Data.ConsumerCommandContracts;
using System.Data;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Stores
{
    public class ActivityTypeStore : IActivityTypeStore
    {
        private readonly IDbConnection _db;

        public ActivityTypeStore(IDbConnection db)
        {
            _db = db;
        }
        public async Task<int> Create(string title)
        {
            var procedure = "dbo.E21_model_modify_segmentation_activity_type";
            var procedureParams = new
            {
                SegmentationActivityTypeTitle = title
            };
            return await _db.QueryFirstOrDefaultAsync<int>(procedure, procedureParams, null, 300, CommandType.StoredProcedure);
        }

        public async Task Update(int id, string title)
        {
            var procedure = "dbo.E21_model_modify_segmentation_activity_type";
            var procedureParams = new
            {
                SegmentationActivityTypeId = id,
                SegmentationActivityTypeTitle = title
            };
            await _db.ExecuteAsync(procedure, procedureParams, null, 300, CommandType.StoredProcedure);
        }
    }
}
