using Dapper;
using LegacySql.Data;
using LegacySql.Data.Models;
using LegacySql.Domain.Cashboxes;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Repositories
{
    public class CashboxApplicationPaymentRepository : ILegacyCashboxApplicationPaymentRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly LegacyDbConnection _legacyDbConnection;

        public CashboxApplicationPaymentRepository(LegacyDbContext db, AppDbContext mapDb, LegacyDbConnection legacyDbConnection)
        {
            _db = db;
            _mapDb = mapDb;
            _legacyDbConnection = legacyDbConnection;
        }

        private async IAsyncEnumerable<CashboxApplicationPayment> GetAllAsync(Expression<Func<CashboxApplicationPaymentEF, bool>> filter, int? take,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var paymentsQuery = _db.CashboxApplicationPayments.Where(e => !e.HeldIn);

            if (filter != null)
            {
                paymentsQuery = paymentsQuery.Where(filter);
            }

#if DEBUG
            take ??= 1000;
#endif
            if (take.HasValue)
            {
                paymentsQuery = paymentsQuery.Take(take.Value);
            }

            var purchasesEf = await paymentsQuery.ToListAsync(cancellationToken);

            await foreach (var i in MapToDomain(purchasesEf, cancellationToken))
            {
                yield return i;
            }
        }

        public async Task<CashboxApplicationPayment> Get(int id, CancellationToken cancellationToken)
        {
            var payment = await _db.CashboxApplicationPayments.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (payment == null)
            {
                return null;
            }

            return await MapToDomain(payment, cancellationToken);
        }

        public async IAsyncEnumerable<CashboxApplicationPayment> GetChangedAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Expression<Func<CashboxApplicationPaymentEF, bool>> filter = PredicateBuilder.New<CashboxApplicationPaymentEF>(true);
            if (changedAt.HasValue)
            {
                filter = filter.And(p => p.Date > changedAt || p.ChangeDate.HasValue && p.ChangeDate > changedAt);
            }

            await foreach (var mappedItem in GetAllAsync(filter, null, cancellationToken))
            {
                yield return mappedItem;
            }

            await foreach (var mappedItem in GetByNotFullMapping(notFullMappingIds, cancellationToken))
            {
                yield return mappedItem;
            }
        }

        private async IAsyncEnumerable<CashboxApplicationPayment> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
                yield break;
            }

            var notFullMappingFilterPortion = 1000;
            var notFullMappingCount = notFullMappingIds.Count();
            var cycleLimitation = (double)notFullMappingCount / notFullMappingFilterPortion;
            for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
            {
                Expression<Func<CashboxApplicationPaymentEF, bool>> filter =
                    p => notFullMappingIds.Skip(i * notFullMappingFilterPortion)
                        .Take(notFullMappingFilterPortion).Any(id => id == p.Id);

                await foreach (var mappedItem in GetAllAsync(filter, null, cancellationToken))
                {
                    yield return mappedItem;
                }
            }
        }

        private async IAsyncEnumerable<CashboxApplicationPayment> MapToDomain(IEnumerable<CashboxApplicationPaymentEF> purchasesEf,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var p in purchasesEf)
            {
                var purchase = await MapToDomain(p, cancellationToken);
                if (purchase != null)
                {
                    yield return purchase;
                }
            }
        }

        private async Task<CashboxApplicationPayment> MapToDomain(CashboxApplicationPaymentEF paymentEf, CancellationToken cancellationToken)
        {
            var writeOffCliectMap = await GetMap<ClientMapEF>(paymentEf.WriteOffCliectId, cancellationToken);
            var receiveClientMap = await GetMap<ClientMapEF>(paymentEf.ReceiveClientId, cancellationToken);
            var paymentEfMap = await _mapDb.CashboxApplicationPaymentMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == paymentEf.Id, cancellationToken);

            return new CashboxApplicationPayment(
                paymentEfMap != null,
                new IdMap(paymentEf.Id, paymentEfMap?.ErpGuid),
                paymentEf.Date,
                writeOffCliectMap,
                receiveClientMap,
                paymentEf.CurrencyId,
                paymentEf.Amount,
                paymentEf.Description,
                paymentEf.ChangeDate
            );
        }

        private async Task<IdMap> GetMap<T>(int id, CancellationToken cancellationToken)
            where T : BaseMapModel
        {
            if (id <= 0)
            {
                return null;
            }
            var mapEf = await _mapDb.Set<T>().AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == id, cancellationToken);
            var map = new IdMap(id, mapEf?.ErpGuid);
            return map;
        }

        public async Task CarryingOutCashboxApplicationPayment(Guid id, Guid incomePaymentId, Guid outPaymentId, Guid userId, DateTime date, bool heldId, CancellationToken cancellationToken)
        {
            var applicationPaymentMapping = await GetMap<CashboxApplicationPaymentMapEF>(id, cancellationToken);
            var incomePaymentMapping = await GetMap<CashboxPaymentMapEF>(incomePaymentId, cancellationToken);
            var outPaymentMapping = await GetMap<CashboxPaymentMapEF>(outPaymentId, cancellationToken);
            var employeeMapping = await GetMap<EmployeeMapEF>(userId, cancellationToken);

            if (applicationPaymentMapping == null)
            {
                throw new KeyNotFoundException("Маппинг заявки не найден");
            }
            if (incomePaymentMapping == null)
            {
                throw new KeyNotFoundException("Маппинг ВКО не найден");
            }
            if (outPaymentMapping == null)
            {
                throw new KeyNotFoundException("Маппинг РКО не найден");
            }
            if (employeeMapping == null)
            {
                throw new KeyNotFoundException("Маппинг сотрудника не найден");
            }

            var updateSqlQuery = @"update [dbo].[TBL_Fin_MoneyTransferApplications] 
                                   set [orderMoveToBalID]=@IncomePaymentId, [orderFromBalID]=@OutPaymentId, [changeUserID]=@UserId, [confirmUserID]=@UserId, [changeDate]=@Date, [heldIn]=@HeldId
                                   where [docID]=@Id";
            await _legacyDbConnection.Connection.ExecuteAsync(updateSqlQuery, new
            {
                Id = applicationPaymentMapping.InnerId,
                IncomePaymentId = incomePaymentMapping.InnerId,
                OutPaymentId = outPaymentMapping.InnerId,
                UserId = employeeMapping.InnerId,
                Date = date,
                HeldId = heldId
            });
        }

        private async Task<IdMap> GetMap<T>(Guid id, CancellationToken cancellationToken)
            where T : BaseMapModel
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            var mapEf = await _mapDb.Set<T>().AsNoTracking()
                .FirstOrDefaultAsync(m => m.ErpGuid == id, cancellationToken);
            
            return mapEf == null ? null : new IdMap(mapEf.LegacyId, mapEf.ErpGuid);
        }
    }
}
