using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.ErpChanged;
using LegacySql.Domain.Extensions;
using LegacySql.Domain.Shared;
using LegacySql.Domain.SupplierCurrencyRates;
using MessageBus.SupplierCurrencyRates.Import;

namespace LegacySql.Consumers.Commands.SupplierCurrencyRates
{
    public class ErpSupplierCurrencyRateSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IErpChangedRepository _erpChangedRepository;

        private ErpSupplierCurrencyRateDto _rate;
        private ExternalMap _clientMapping;
        private int? rateId = null;

        public ErpSupplierCurrencyRateSaver(IDbConnection db,  
            IClientMapRepository clientMapRepository, 
            IErpChangedRepository erpChangedRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _erpChangedRepository = erpChangedRepository;
        }

        public void InitErpObject(ErpSupplierCurrencyRateDto rate)
        {
            _rate = rate;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            _clientMapping = await _clientMapRepository.GetByErpAsync(_rate.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{_rate.ClientId} не найден\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task SaveErpObject(Guid messageId)
        {
            var selectClientRateIdSqlQuery = @"select [ID] from [dbo].[kurses]
                                            where [klientID]=@ClientId";
            rateId = await _db.QueryFirstOrDefaultAsync<int>(selectClientRateIdSqlQuery, new
            {
                ClientId = _clientMapping.LegacyId
            });

            if (!rateId.HasValue)
            {
                await Create();
            }
            else
            {
                await Update();
            }
        }

        private async Task Update()
        {
            var updateSqlQuery = @"update [dbo].[kurses] 
                                 set [partner]='erp',[kursNal]=@RateNal,[kursBN]=@RateBn,[kursDDP]=@RateDdr,[ДатаИзм]=@Date
                                 where [ID]=@Id";
            var changeDate = DateTime.Now.RoundUpToMillisecond();
            await _db.ExecuteAsync(updateSqlQuery, new
            {
                Id=rateId,
                RateNal = _rate.RateNal,
                RateBn = _rate.RateBn,
                RateDdr = _rate.RateDdr,
                Date = changeDate
            });

            await _erpChangedRepository.Save(
                rateId.Value,
                changeDate,
                typeof(SupplierCurrencyRate).Name
            );
        }

        private async Task Create()
        {
            var insertSqlQuery = @"insert into [dbo].[kurses] 
                                 ([partner],[kursNal],[kursBN],[kursDDP],[ДатаИзм],[klientID])
                                 values ('erp',@RateNal,@RateBn,@RateDdr,@Date,@ClientId);
                                 select cast(SCOPE_IDENTITY() as int)";
            var changeDate = DateTime.Now.RoundUpToMillisecond();
            var newId = (await _db.QueryAsync<int>(insertSqlQuery, new
            {
                RateNal = _rate.RateNal,
                RateBn = _rate.RateBn,
                RateDdr = _rate.RateDdr,
                Date = changeDate,
                ClientId = _clientMapping.LegacyId
            })).FirstOrDefault();

            await _erpChangedRepository.Save(
                newId,
                changeDate,
                typeof(SupplierCurrencyRate).Name
            );
        }
    }
}
