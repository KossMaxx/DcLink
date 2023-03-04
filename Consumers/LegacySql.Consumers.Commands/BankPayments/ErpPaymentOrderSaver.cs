using Dapper;
using LegacySql.Domain.BankPayments;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using MessageBus.BankPayments.Import;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.BankPayments
{
    public class ErpPaymentOrderSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IPaymentOrderMapRepository _paymentOrderMapRepository;
        private ErpPaymentOrderDto _payment;
        private ExternalMap _paymentMapping;
        private ExternalMap _clientMapping;

        public ErpPaymentOrderSaver(IDbConnection db, IClientMapRepository clientMapRepository, IPaymentOrderMapRepository paymentOrderMapRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _paymentOrderMapRepository = paymentOrderMapRepository;
        }

        public void InitErpObject(ErpPaymentOrderDto entity, ExternalMap paymentMapping)
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

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Create(Guid messageGuid)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var firmInfo = await GetFirm(_payment.Okpo, transaction);

                var insertQuery = @"insert into [dbo].[platezka]
                                       ([data_pl],[n_pp],[name],[N_pnDoc],[sum],[nazn],[OKPO],[schet],[mfo],[FirmID],[klientID],[kurs],[balOK],[company])
                                       values (@Date,@OrderNumber,@FirmName,@InnerDocNumber,@Sum,@Purpose,@Okpo,@Account,@Mfo,@FirmId,@ClientId,@Rate,@IsConfirmed,@Company);
                                       select cast(SCOPE_IDENTITY() as int)";
                var newPaymentId = (await _db.QueryAsync<int>(insertQuery, new
                {
                    Date = _payment.Date,
                    OrderNumber = _payment.OrderNumber,
                    FirmName = firmInfo.Name,
                    InnerDocNumber = _payment.InnerDocNumber,
                    Sum = _payment.Sum,
                    Purpose = _payment.Purpose,
                    Okpo = _payment.Okpo,
                    Company = await GetCompanyId(_payment.Company, _payment.CurrencyId, transaction),
                    Account = _payment.Account,
                    FirmId = firmInfo.Id,
                    Mfo = _payment.Mfo,
                    ClientId = _clientMapping.LegacyId,
                    Rate = _payment.Rate,
                    IsConfirmed = _payment.IsConfirmed,
                }, transaction)).FirstOrDefault();

                if (_payment.PurchaseId.HasValue)
                {
                    await SetRelations(newPaymentId, transaction);
                }

                transaction.Commit();

                await _paymentOrderMapRepository.SaveAsync(new ExternalMap(messageGuid, newPaymentId, _payment.Id));
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _db.Close();
            }
        }

        private async Task SetRelations(int newPaymentId, IDbTransaction transaction)
        {
            var selectBillIdQuery = @"select bill.Код_счета as Id,[Номер] Number from [dbo].[ПН] pn
                                        right join [dbo].[connected_documents] cd 
                                        on (type1=11 and type2=3 and doc2ID in (pn.НомерПН))
                                        right join [dbo].[СчетПН] bill on Код_счета = cd.doc1ID
                                        where pn.НомерПН=@PurchaseId";
            var billInfo = await _db.QueryAsync<BillInfo>(selectBillIdQuery, new
            {
                PurchaseId = _payment.PurchaseId
            }, transaction);

            if (!billInfo.Any())
            {
                return;
            }

            var insertRelationQuery = @"insert into [dbo].[connected_documents]  
                                        ([type1],[type2],[doc1ID],[doc2ID],[zu],[zt])
                                        values (11,7,@BillId,@PaymentOrderId,@Username,@Data)";
            await _db.ExecuteAsync(insertRelationQuery, new
            {
                BillId = billInfo.First().Id,
                PaymentOrderId = newPaymentId,
                Username = _payment.Username,
                Data = _payment.Date
            }, transaction);

            var updatePaymentOrderQuery = @"update [dbo].[platezka] set [N_pnDoc]=@BillId
                                            where [bank_ID]=@PaymentOrderId";
            await _db.ExecuteAsync(updatePaymentOrderQuery, new {
                BillId = billInfo.First().Id,
                PaymentOrderId = newPaymentId
            }, transaction);
        }

        public async Task Update()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var firmInfo = await GetFirm(_payment.Okpo, transaction);

                var updateQuery = @"update [dbo].[platezka] set
                                       [data_pl]=@Date,
                                       [n_pp]=@OrderNumber,
                                       [name]=@FirmName,
                                       [N_pnDoc]=@InnerDocNumber,
                                       [sum]=@Sum,
                                       [nazn]=@Purpose,
                                       [OKPO]=@Okpo,
                                       [schet]=@Account,
                                       [mfo]=@Mfo,
                                       [FirmID]=@FirmId,
                                       [klientID]=@ClientId,
                                       [kurs]=@Rate,
                                       [balOK]=@IsConfirmed,
                                       [company]=@Company
                                       where [bank_ID]=@Id";
                await _db.ExecuteAsync(updateQuery, new
                {
                    Id = _paymentMapping.LegacyId,
                    Date = _payment.Date,
                    OrderNumber = _payment.OrderNumber,
                    FirmName = firmInfo.Name,
                    InnerDocNumber = _payment.InnerDocNumber,
                    Sum = _payment.Sum,
                    Purpose = _payment.Purpose,
                    Okpo = _payment.Okpo,
                    Company = await GetCompanyId(_payment.Company, _payment.CurrencyId, transaction),
                    Account = _payment.Account,
                    FirmId = firmInfo.Id,
                    Mfo = _payment.Mfo,
                    ClientId = _clientMapping.LegacyId,
                    Rate = _payment.Rate,
                    IsConfirmed = _payment.IsConfirmed,
                }, transaction);

                await ChangeRelations(transaction);

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _db.Close();
            }
        }

        private async Task ChangeRelations(IDbTransaction transaction)
        {
            var selectCurrentRelationQuery = @"select * from [dbo].[connected_documents]  
                                              where [type1]=11 and [type2]=7 and [doc2ID]=@PaymentOrderId";

            var currentRelation = await _db.QueryFirstOrDefaultAsync<ConnectedDocumentsEF>(selectCurrentRelationQuery, new
            {
                PaymentOrderId = _paymentMapping.LegacyId
            }, transaction);

            if (currentRelation != null)
            {
                var deleteRelationQuery = @"delete from [dbo].[connected_documents]  
                                            where [id]=@Id";
                await _db.ExecuteAsync(deleteRelationQuery, new
                {
                    Id = currentRelation.Id
                }, transaction);
            }

            if (_payment.PurchaseId.HasValue)
            {
                await SetRelations(_paymentMapping.LegacyId, transaction);
            }
            else
            {
                var updatePaymentOrderQuery = @"update [dbo].[platezka] set [N_pnDoc]=null
                                                where [bank_ID]=@PaymentOrderId";
                await _db.ExecuteAsync(updatePaymentOrderQuery, new
                {
                    PaymentOrderId = _paymentMapping.LegacyId
                }, transaction);
            }
        }

        private async Task<FirmInfo> GetFirm(string okpo, IDbTransaction transaction)
        {
            var selectFirmQuery = @"select [Код] as Id, [Название] as Name from [dbo].[Firms]
                                    where [ОКПО]=@Okpo";
            var firmId = await _db.QueryFirstOrDefaultAsync<FirmInfo>(selectFirmQuery, new
            {
                Okpo = okpo
            }, transaction);

            return firmId;
        }

        private async Task<int> GetCompanyId(string company, int currencyId, IDbTransaction transaction)
        {
            var selectCompanyQuery = @"select [bankListID] from [dbo].[BankList]
                                       where [rs]=@Company and [currency_id]=@CurrencyId";

            var companyId = await _db.QueryFirstOrDefaultAsync<int?>(selectCompanyQuery, new
            {
                Company = company,
                CurrencyId = currencyId
            }, transaction);

            return companyId ?? 0;
        }
    }
}
