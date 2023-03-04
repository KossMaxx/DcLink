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
    public class CashboxPaymentRepository : ILegacyCashboxPaymentRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;

        public CashboxPaymentRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        private async IAsyncEnumerable<CashboxPayment> GetAllAsync(Expression<Func<CashboxPaymentEF, bool>> filter, int? take,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var paymentsQuery = _db.CashboxPayments.AsQueryable();

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

        public async IAsyncEnumerable<CashboxPayment> GetChangedAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Expression<Func<CashboxPaymentEF, bool>> filter = PredicateBuilder.New<CashboxPaymentEF>(true);
            if (changedAt.HasValue)
            {
                filter = filter.And(p => p.CreateDate.HasValue && p.CreateDate > changedAt || p.ChangeDate.HasValue && p.ChangeDate > changedAt);
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

        private async IAsyncEnumerable<CashboxPayment> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
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
                Expression<Func<CashboxPaymentEF, bool>> filter =
                    p => notFullMappingIds.Skip(i * notFullMappingFilterPortion)
                        .Take(notFullMappingFilterPortion).Any(id => id == p.Id);

                await foreach (var mappedItem in GetAllAsync(filter, null, cancellationToken))
                {
                    yield return mappedItem;
                }
            }
        }

        private async IAsyncEnumerable<CashboxPayment> MapToDomain(IEnumerable<CashboxPaymentEF> purchasesEf,
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

        private async Task<CashboxPayment> MapToDomain(CashboxPaymentEF paymentEf, CancellationToken cancellationToken)
        {
            var cashboxMap = await GetMap<CashboxMapEF>(paymentEf.CashboxId, cancellationToken);
            var clientMap = await GetMap<ClientMapEF>(paymentEf.ClientId, cancellationToken);

            var paymentEfMap = await _mapDb.CashboxPaymentMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == paymentEf.Id, cancellationToken);

            return new CashboxPayment(
                paymentEfMap != null,
                new IdMap(paymentEf.Id, paymentEfMap?.ErpGuid),
                paymentEf.Date,
                cashboxMap,
                clientMap,
                paymentEf.AmountUSD,
                paymentEf.AmountUAH,
                paymentEf.AmountEuro,
                paymentEf.Rate,
                paymentEf.RateEuro,
                paymentEf.Description,
                total: paymentEf.AmountUSD + (paymentEf.AmountUAH / paymentEf.Rate) + (paymentEf.AmountEuro / paymentEf.RateEuro),
                changedAt: paymentEf.CreateDate ?? paymentEf.ChangeDate
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

        public async Task<CashboxPayment> Get(int id, CancellationToken cancellationToken)
        {
            var payment = await _db.CashboxPayments.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if(payment == null)
            {
                return null;
            }

            return await MapToDomain(payment, cancellationToken);
        }
    }
}
