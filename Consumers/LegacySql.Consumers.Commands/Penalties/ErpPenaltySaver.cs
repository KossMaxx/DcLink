using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Shared;
using MessageBus.Penalties.Import;

namespace LegacySql.Consumers.Commands.Penalties
{
    public class ErpPenaltySaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private ErpPenaltyDto _penalty;
        private ExternalMap _clientMapping;

        public ErpPenaltySaver(IDbConnection db, 
            IClientMapRepository clientMapRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
        }

        public void InitErpObject(ErpPenaltyDto penalty)
        {
            _penalty = penalty;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            _clientMapping = await _clientMapRepository.GetByErpAsync(_penalty.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{_penalty.ClientId} не найден\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Create()
        {
            var insertQuery = @"insert into [dbo].[Penya] 
                                ([klientID],[dsozd],[sss])
                                values (@ClientId,@Date,@Sum);
                                select cast(SCOPE_IDENTITY() as int)";

            await _db.ExecuteAsync(insertQuery, new
            {
                ClientId = _clientMapping.LegacyId,
                _penalty.Date,
                _penalty.Sum,
            });
        }
    }
}
