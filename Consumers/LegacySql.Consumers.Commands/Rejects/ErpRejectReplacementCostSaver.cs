using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Rejects;
using LegacySql.Domain.Shared;
using MessageBus.Rejects.Import;

namespace LegacySql.Consumers.Commands.Rejects
{
    public class ErpRejectReplacementCostSaver
    {
        private readonly IDbConnection _db;
        private readonly IRejectMapRepository _rejectMapRepository;
        private ErpRejectReplacementCostDto _erpRejectReplacementCost;
        private ExternalMap _rejectMapping;

        public ErpRejectReplacementCostSaver(IDbConnection db, IRejectMapRepository rejectMapRepository)
        {
            _db = db;
            _rejectMapRepository = rejectMapRepository;
        }

        public void InitErpObject(ErpRejectReplacementCostDto cost)
        {
            _erpRejectReplacementCost = cost;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            _rejectMapping = await _rejectMapRepository.GetByErpAsync(_erpRejectReplacementCost.RejectId);
            if (_rejectMapping == null)
            {
                why.Append($"Не найден маппинг для RejectId:{_erpRejectReplacementCost.RejectId}\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task SaveErpObject()
        {
            var saveSqlQuery = @"update [dbo].[brak]
                                cost1=@ReplacementCost
                                where Brak_ID=@RejectId";

            await _db.ExecuteAsync(saveSqlQuery, new
            {
                ReplacementCost = _erpRejectReplacementCost.ReplacementCost,
                RejectId = _rejectMapping.LegacyId
            });
        }
    }
}
