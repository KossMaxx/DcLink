using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Shared;
using MessageBus.Bonuses.Import;

namespace LegacySql.Consumers.Commands.Bonuses
{
    public class ErpBonusSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private ErpBonusDto _bonus;
        private ExternalMap _clientMapping;

        public ErpBonusSaver(IClientMapRepository clientMapRepository, 
            IDbConnection db)
        {
            _clientMapRepository = clientMapRepository;
            _db = db;
        }

        public void InitErpObject(ErpBonusDto bonus)
        {
            _bonus = bonus;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            _clientMapping = await _clientMapRepository.GetByErpAsync(_bonus.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{_bonus.ClientId} не найден\n");
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
                _bonus.Date,
                _bonus.Sum,
            });
        }
    }
}
