using Dapper;
using LegacySql.Legacy.Data.ConsumerCommandContracts;
using System.Data;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Stores
{
    public class PartnerProductGroupsStore : IPartnerProductGroupsStore
    {
        private readonly IDbConnection _db;

        public PartnerProductGroupsStore(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> Create(string title)
        {
            var procedure = "dbo.E21_model_modify_segmentation_product_group";
            var procedureParams = new
            {
                SegmentationProductGroupsTitle = title
            };
            return await _db.QueryFirstOrDefaultAsync<int>(procedure, procedureParams, null, 300, CommandType.StoredProcedure);
        }

        public async Task Update(int id, string title)
        {
            var procedure = "dbo.E21_model_modify_segmentation_product_group";
            var procedureParams = new
            {
                SegmentationProductGroupsId = id,
                SegmentationProductGroupsTitle = title
            };
            await _db.ExecuteAsync(procedure, procedureParams, null, 300, CommandType.StoredProcedure);
        }
    }
}
