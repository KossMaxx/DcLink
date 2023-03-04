using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Cashboxes;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Shared;
using MessageBus.Cashboxes.Import;

namespace LegacySql.Consumers.Commands.Cashboxes
{
    public class ErpCashboxPaymentSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly ICashboxPaymentMapRepository _cashboxPaymentMapRepository;
        private readonly ICashboxMapRepository _cashboxMapRepository;
        private ExternalMap _paymentMapping;
        private ExternalMap _clientMapping;
        private CashboxMap _cashboxMapping;
        private ErpCashboxPaymentDto _payment;

        public ErpCashboxPaymentSaver(IDbConnection db,
            IClientMapRepository clientMapRepository,
            ICashboxPaymentMapRepository cashboxPaymentMapRepository, 
            ICashboxMapRepository cashboxMapRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _cashboxPaymentMapRepository = cashboxPaymentMapRepository;
            _cashboxMapRepository = cashboxMapRepository;
        }

        public void InitErpObject(ErpCashboxPaymentDto entity, ExternalMap paymentMapping)
        {
            _payment = entity;
            _paymentMapping = paymentMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            _clientMapping = await _clientMapRepository.GetByErpAsync(_payment.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{_payment.ClientId} не найден\n");
            }

            _cashboxMapping = await _cashboxMapRepository.GetByErpAsync(_payment.CashboxId);
            if (_cashboxMapping == null)
            {
                why.Append($"Маппинг кассы id:{_payment.CashboxId} не найден\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Create(Guid messageGuid)
        {
             var insertQuery = @"insert into [dbo].[Касса]
                                       ([грн],
                                        [курс],
                                        [дата],
                                        [Прим],
                                        [klientID],  
                                        [Kassa_ID],
                                        [transit],
                                        [ден],
                                        [евро],
                                        [курсевро],
                                        [sozdal],
                                        [dsozd],
                                        [lastuser],
                                        [lasttime],
                                        [balOK])
                                       values (
                                        @Amount,
                                        @Rate,
                                        @Date,
                                        @Description,
                                        @ClientId,
                                        @CashboxId,
                                        @IsTransit,
                                        @AmountUsd,
                                        @AmountEuro,
                                        @RateEuro,
                                        @Creator,
                                        @CreateDate,
                                        @Updator,
                                        @UpdateDate,
                                        @IsConfirmed
                                        );
                                       select cast(SCOPE_IDENTITY() as int)";
                var newPaymentId = (await _db.QueryAsync<int>(insertQuery, new
                {
                    Amount = _payment.AmountUah,
                    Rate = _payment.Rate,
                    Date = _payment.Date,
                    Description = _payment.Description,
                    ClientId = _clientMapping.LegacyId,
                    CashboxId = _cashboxMapping.LegacyId,
                    IsTransit = _payment.IsTransit,
                    AmountUsd = _payment.AmountUsd,
                    AmountEuro = _payment.AmountEuro,
                    RateEuro = _payment.RateEuro == 0 ? await GetCrossRateEuro() : _payment.RateEuro,
                    Creator = _payment.CreatorUsername,
                    CreateDate = _payment.CreateDate,
                    Updator = _payment.UpdatorUsername,
                    UpdateDate = _payment.UpdateDate,
                    IsConfirmed = _payment.IsConfirmed
                })).FirstOrDefault();

                await _cashboxPaymentMapRepository.SaveAsync(new ExternalMap(messageGuid, newPaymentId, _payment.Id));

        }

        public async Task Update()
        {
            var updateQuery = @"update [dbo].[Касса] set
                                        [грн]=@Amount,
                                        [курс]=@Rate,
                                        [дата]=@Date,
                                        [Прим]=@Description,
                                        [klientID]=@ClientId,
                                        [Kassa_ID]=@CashboxId,
                                        [transit]=@IsTransit,
                                        [ден]=@AmountUsd,
                                        [евро]=@AmountEuro,
                                        [курсевро]=@RateEuro,
                                        [lastuser]=@Updator,
                                        [lasttime]=@UpdateDate,
                                        [balOK]=@IsConfirmed
                                       where [код]=@Id";
            await _db.ExecuteAsync(updateQuery, new
            {
                Id = _paymentMapping.LegacyId,
                Amount = _payment.AmountUah,
                Rate = _payment.Rate,
                Date = _payment.Date,
                Description = _payment.Description,
                ClientId = _clientMapping.LegacyId,
                CashboxId = _cashboxMapping.LegacyId,
                IsTransit = _payment.IsTransit,
                AmountUsd = _payment.AmountUsd,
                AmountEuro = _payment.AmountEuro,
                RateEuro = _payment.RateEuro == 0 ? await GetCrossRateEuro() : _payment.RateEuro,
                Updator = _payment.UpdatorUsername,
                UpdateDate = _payment.UpdateDate,
                IsConfirmed = _payment.IsConfirmed
            });
        }

        private async Task<decimal> GetCrossRateEuro()
        {
            var selectQuery = @"select [евро]
                                from [dbo].[Курс]
                                where [Дата]= (select max( [Дата] ) from [dbo].[Курс])";
            var rate = await _db.QueryFirstOrDefaultAsync<decimal>(selectQuery);
            return rate;
        }
    }
}
