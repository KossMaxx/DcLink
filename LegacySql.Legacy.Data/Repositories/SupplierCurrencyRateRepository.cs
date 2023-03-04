using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Shared;
using LegacySql.Domain.SupplierCurrencyRates;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class SupplierCurrencyRateRepository : ILegacySupplierCurrencyRateRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;

        public SupplierCurrencyRateRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        public async IAsyncEnumerable<SupplierCurrencyRate> GetChangedSupplierCurrencyRatesAsync(DateTime? changedAt, [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            if (changedAt.HasValue)
            {
                await foreach (var item in GetAllAsync(p => p.Date.HasValue && p.Date > changedAt, cancellationToken))
                {
                    yield return item;
                }
                yield break;
            }

            await foreach (var rate in GetAllAsync(null, cancellationToken))
            {
                yield return rate;
            }
        }

        public async IAsyncEnumerable<SupplierCurrencyRate> GetSupplierCurrencyRateAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in GetAllAsync(p => p.ClientId == id, cancellationToken))
            {
                yield return item;
            }
        }

        private async IAsyncEnumerable<SupplierCurrencyRate> GetAllAsync(Expression<Func<SupplierCurrencyRateEF, bool>> filter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var query = _db.SupplierCurrencyRates
                .Include(e=>e.Client)
                .Where(e=>e.Date.HasValue
                    && ((e.RateBn.HasValue && e.RateBn != 1)
                    || (e.RateNal.HasValue && e.RateNal != 1)))
                .AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var ratesEf = await query
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken: cancellationToken);

            foreach (var item in ratesEf)
            {
                yield return await MapToDomain(item, cancellationToken);
            }
        }

        private async Task<SupplierCurrencyRate> MapToDomain(SupplierCurrencyRateEF rate, CancellationToken cancellationToken)
        {
            var clientMap = rate.ClientId.HasValue
                ? await _mapDb.ClientMaps.FirstOrDefaultAsync(e => e.LegacyId == rate.ClientId, cancellationToken)
                : null;


            return new SupplierCurrencyRate(
                rate.Id,
                rateNal:rate.RateNal,
                rateBn:rate.RateBn,
                rateDdr:rate.RateDdr,
                date:rate.Date,
                clientId: rate.ClientId.HasValue ? new IdMap(rate.ClientId.Value, clientMap?.ErpGuid) : null,
                changedByBot:rate.Partner.Trim() == "--бот--",
                balanceCurrencyId: rate.Client?.BalanceCurrencyId,
                isSupplier: rate.Client?.IsSupplier,
                isCustomer: rate.Client?.IsCustomer
            );
        }
    }
}
