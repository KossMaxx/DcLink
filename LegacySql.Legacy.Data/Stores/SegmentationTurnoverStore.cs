using Dapper;
using LegacySql.Legacy.Data.ConsumerCommandContracts;
using System.Data;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Stores
{
    public class SegmentationTurnoverStore : ISegmentationTurnoverStore
    {
        private readonly IDbConnection _db;

        public SegmentationTurnoverStore(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Create(string title)
        {
            var procedure = "dbo.E21_model_modify_segmentation_turnover";
            var procedureParams = new
            {
                SegmentationTurnoverTitle = title
            };
            return await _db.QueryFirstOrDefaultAsync<int>(procedure, procedureParams, null, 300, CommandType.StoredProcedure);
        }

        public async Task Update(int id, string title)
        {
            var procedure = "dbo.E21_model_modify_segmentation_turnover";
            var procedureParams = new
            {
                SegmentationTurnoverId = id,
                SegmentationTurnoverTitle = title
            };
            await _db.ExecuteAsync(procedure, procedureParams, null, 300, CommandType.StoredProcedure);
        }
    }
}
