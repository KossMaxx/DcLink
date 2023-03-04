using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.BankPayments;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using MessageBus.BankPayments.Import;

namespace LegacySql.Consumers.Commands.BankPayments
{
    public class ErpBankPaymentSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IBankPaymentMapRepository _bankPaymentMapRepository;
        private ErpBankPaymentDto _payment;
        private BankPaymentMap _paymentMapping;
        private Dictionary<Guid, int> _clientMappings;
        private ErpBankPaymentClientOrderDto _clientOrder;

        public ErpBankPaymentSaver(IClientMapRepository clientMapRepository,
            IDbConnection db,
            IBankPaymentMapRepository bankPaymentMapRepository)
        {
            _clientMapRepository = clientMapRepository;
            _db = db;
            _bankPaymentMapRepository = bankPaymentMapRepository;
            _clientMappings = new Dictionary<Guid, int>();
        }

        public void InitErpObject(ErpBankPaymentDto entity, BankPaymentMap paymentMapping, ErpBankPaymentClientOrderDto clientOrder)
        {
            _payment = entity;
            _paymentMapping = paymentMapping;
            _clientOrder = clientOrder;
        }

        public async Task<MappingInfo> GetMappingInfo(Guid clientId)
        {
            var why = new StringBuilder();
            var clientMapping = await _clientMapRepository.GetByErpAsync(clientId);
            if (clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{clientId} не найден\n");
            }
            else
            {
                if (!_clientMappings.ContainsKey(clientId))
                {
                    _clientMappings.Add(clientId, clientMapping.LegacyId);
                }
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
                var firmId = await GetFirmId(_payment.Okpo, transaction);

                var insertQuery = @"insert into [dbo].[bank_prihod]
                                       ([sum],[kurs],[дата],[прим],[klientID],[balOKuser],[n_pp],[FirmID],[company],[balOK])
                                       values (@Amount,@Rate,@Date,@Description,@ClientId,@Username,@BankNumber,@FirmId,@Company,@IsConfirmed);
                                       select cast(SCOPE_IDENTITY() as int)";
                var newPaymentId = (await _db.QueryAsync<int>(insertQuery, new
                {
                    Amount = _clientOrder.Amount,
                    Rate = _payment.Rate,
                    Date = _payment.Date,
                    Description = _payment.Description,
                    ClientId = _clientMappings[_clientOrder.ClientId],
                    Username = _payment.Username,
                    BankNumber = _payment.BankNumber,
                    //BillCode = _payment.BillCode,
                    //BillNumber = _payment.BillNumber,
                    FirmId = firmId,
                    Company = await GetCompanyId(_payment.Company, _payment.CurrencyId, transaction),
                    IsConfirmed = _payment.IsConfirmed
                }, transaction)).FirstOrDefault();

                if (_clientOrder?.ClientOrderId != null)
                {
                    await SetRelations(newPaymentId, firmId, _clientOrder.IsBill, transaction);
                }

                transaction.Commit();

                if (!_clientOrder.ClientOrderId.HasValue)
                {
                    await _bankPaymentMapRepository.SaveAsync(new BankPaymentMap(messageGuid, newPaymentId, null, _payment.Id));
                }
                else
                {
                    await _bankPaymentMapRepository.SaveAsync(new BankPaymentMap(messageGuid, newPaymentId, _clientOrder.ClientOrderId.Value, _payment.Id));
                }                
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

        private async Task SetRelations(int newPaymentId, int? firmId, bool isBill, IDbTransaction transaction)
        {
            BillInfo bill;
            if (!isBill)
            {
                var selectBillIdQuery = @"select bill.Код_счета as Id,[Номер] as Number, bill.[дата] as Date, [OOO] as CompanyId from [dbo].[РН] rn
                                     right join [dbo].[connected_documents] cd 
                                     on (type1=1 and type2=10 and doc1ID in (rn.НомерПН))
                                     right join [dbo].[Счет] bill on Код_счета = cd.doc2ID
                                     where rn.НомерПН=@ClientOrderId and (bill.оплачен is null or bill.оплачен = 0)";
                var billInfo = await _db.QueryAsync<BillInfo>(selectBillIdQuery, new
                {
                    ClientOrderId = _clientOrder.ClientOrderId
                }, transaction);

                if (!billInfo.Any())
                {
                    return;
                }

                bill = billInfo.First();
            }
            else
            {
                var selectBillInfoQuery = @"select [Код_счета] as Id,[Номер] as Number, [дата] as Date, [OOO] as CompanyId, [FirmID] as FirmId from [dbo].[Счет]
                                            where [Код_счета]=@BillId";
                bill = await _db.QueryFirstOrDefaultAsync<BillInfo>(selectBillInfoQuery, new
                {
                    BillId = _clientOrder.ClientOrderId
                }, transaction);
            }
            //var taxInvoice = await GetTaxInvoice(bill.Id, bill.CompanyId, bill.Date, transaction);

            if (!_payment.IsLiqpay)
            {
                var updateBillQuery = @"update [dbo].[Счет] 
                                        set 
                                        [оплачен] = 1, 
                                        оплаченЮ = @Username, 
                                        [FirmID] = @FirmId
                                        where [Код_счета]=@BillId";
                await _db.ExecuteAsync(updateBillQuery, new
                {
                    BillId = bill.Id,
                    Username = _payment.Username,
                    FirmId = !bill.FirmId.HasValue ? firmId : bill.FirmId
                    //TaxInvoice = taxInvoice
                }, transaction);
            }

            if (_payment.IsLiqpay)
            {
                var updateBillQuery = @"update [dbo].[Счет] 
                                      set 
                                      [оплачен] = 1, 
                                      [оплаченЮ] = @Username, 
                                      [FirmID] = @FirmId, 
                                      [выдан] = 1, 
                                      [НомерН] = @ClientOrderId, 
                                      [датаН]=@BillDate
                                      where [Код_счета]=@BillId";
                await _db.ExecuteAsync(updateBillQuery, new
                {
                    BillId = bill.Id,
                    Username = _payment.Username,
                    FirmId = firmId,
                    ClientOrderId = bill.Number,
                    BillDate = _payment.Date
                }, transaction);
            }

            var insertRelationQuery = @"insert into [dbo].[connected_documents]  
                                        ([type1],[type2],[doc1ID],[doc2ID],[zu],[zt])
                                        values (10,6,@BillId,@BankPaymentId,@Username,@Date)";
            await _db.ExecuteAsync(insertRelationQuery, new
            {
                BillId = bill.Id,
                BankPaymentId = newPaymentId,
                Username = _payment.Username,
                Date = _payment.Date
            }, transaction);

            var updateBankPaymentQuery = @"update [dbo].[bank_prihod] set [InvID]=@BillId,[счет]=@BillNumber
                                           where [bank_ID]=@BankPaymentId";
            await _db.ExecuteAsync(updateBankPaymentQuery, new
            {
                BillId = bill.Id,
                BillNumber = bill.Number,
                BankPaymentId = newPaymentId
            }, transaction);
        }

        public async Task Update()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var firmId = await GetFirmId(_payment.Okpo, transaction);

                var updateQuery = @"update [dbo].[bank_prihod] set
                                       [sum]=@Amount,
                                       [kurs]=@Rate,
                                       [дата]=@Date,
                                       [прим]=@Description,
                                       [klientID]=@ClientId,
                                       [balOKuser]=@Username,
                                       [n_pp]=@BankNumber,
                                       [FirmID]=@FirmId,
                                       [company]=@Company,
                                       [balOK]=@IsConfirmed
                                       where [bank_ID]=@Id";
                await _db.ExecuteAsync(updateQuery, new
                {
                    Id = _paymentMapping.LegacyId,
                    Amount = _clientOrder.Amount,
                    Rate = _payment.Rate,
                    Date = _payment.Date,
                    Description = _payment.Description,
                    ClientId = _clientMappings[_clientOrder.ClientId],
                    Username = _payment.Username,
                    BankNumber = _payment.BankNumber,
                    //BillCode = _payment.BillCode,
                    //BillNumber = _payment.BillNumber,
                    FirmId = firmId,
                    Company = await GetCompanyId(_payment.Company, _payment.CurrencyId, transaction),
                    IsConfirmed = _payment.IsConfirmed
                }, transaction);

                await ChangeRelations(firmId, transaction);

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

        private async Task ChangeRelations(int? firmId, IDbTransaction transaction)
        {
            var selectCurrentRelationQuery = @"select * from [dbo].[connected_documents]  
                                              where [type1]=10 and [type2]=6 and [doc2ID]=@BankPaymentId";

            var currentRelation = await _db.QueryFirstOrDefaultAsync<ConnectedDocumentsEF>(selectCurrentRelationQuery, new
            {
                BankPaymentId = _paymentMapping.LegacyId
            }, transaction);

            if(currentRelation != null)
            {
                var updateBillQuery = @"update [dbo].[Счет] set [оплачен] = 0, оплаченЮ = null, [FirmID] = null, [выдан] = null, [НомерН] = null, [датаН]=null, [датаНН]=null, [NNom]=null
                                        where [Код_счета]=@BillId";
                await _db.ExecuteAsync(updateBillQuery, new
                {
                    BillId = currentRelation.Doc1Id,
                }, transaction);

                var deleteRelationQuery = @"delete from [dbo].[connected_documents]  
                                            where [id]=@Id";
                await _db.ExecuteAsync(deleteRelationQuery, new
                {
                    Id = currentRelation.Id
                }, transaction);
            }           

            if (_clientOrder?.ClientOrderId != null)
            {
                await SetRelations(_paymentMapping.LegacyId, firmId, _clientOrder.IsBill, transaction);
            }
            else
            {
                var updateBankPaymentQuery = @"update [dbo].[bank_prihod] set [InvID]=null,[счет]=null,[sum]=0
                                               where [bank_ID]=@BankPaymentId";
                await _db.ExecuteAsync(updateBankPaymentQuery, new
                {
                    BankPaymentId = _paymentMapping.LegacyId
                }, transaction);
            }
        }

        private async Task<int?> GetFirmId(string okpo, IDbTransaction transaction)
        {
            var selectFirmQuery = @"select [Код] from [dbo].[Firms]
                                    where [ОКПО]=@Okpo";
            var firmId = await _db.QueryFirstOrDefaultAsync<int?>(selectFirmQuery, new
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

        public async Task DeletePaymentOrder()
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var firmId = await GetFirmId(_payment.Okpo, transaction);
                await ChangeRelations(firmId, transaction);

                transaction.Commit();

                await _bankPaymentMapRepository.SaveAsync(
                    new BankPaymentMap(
                        _paymentMapping.MapId, 
                        _paymentMapping.LegacyId, 
                        null, 
                        _paymentMapping.ExternalMapId), 
                        _paymentMapping.Id
                        );
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

        public async Task DeletePaymentOrderWithoutClientOrder(IEnumerable<int> paymentIdsWithoutClientOrder)
        {
            var updateBankPaymentQuery = @"update [dbo].[bank_prihod] set [sum]=@Amount
                                           where [bank_ID]=@BankPaymentId";
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                foreach (var paymentId in paymentIdsWithoutClientOrder)
                {
                    await _db.ExecuteAsync(updateBankPaymentQuery, new
                    {
                        BankPaymentId = paymentId,
                        Amount = 0
                    }, transaction);
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _db.Close();
            }
        }
        //private async Task<int> GetTaxInvoice(int billId, int? companyId, DateTime? date, IDbTransaction transaction)
        //{
        //    if(!companyId.HasValue || companyId == 0 || !date.HasValue)
        //    {
        //        return 0;
        //    }

        //    var currentDate = new DateTime(date.Value.Year, date.Value.Month, 1);
        //    var selectQuery = @"select [NNom] from [dbo].[Счет]
        //                        where [Код_счета] <> @BillId and [OOO]=@CompanyId and [датаНН] > @DateBefore and [датаНН] < @DateAfter 
        //                        and  [NNom] = (select max([NNom]) from [dbo].[Счет] where [Код_счета] <> @BillId and [OOO]=@CompanyId and [датаНН] > @DateBefore and [датаНН] < @DateAfter)";
        //    var maxTaxInvoice = await _db.QueryFirstOrDefaultAsync<int?>(selectQuery, new
        //    {
        //        BillId = billId,
        //        CompanyId = companyId,
        //        DateBefore = currentDate.AddDays(-1),
        //        DateAfter = currentDate.AddMonths(1)
        //    }, transaction);

        //    return !maxTaxInvoice.HasValue ? 1 : maxTaxInvoice.Value + 1;
        //}
    }
}
