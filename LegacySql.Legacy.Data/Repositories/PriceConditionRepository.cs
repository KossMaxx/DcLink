using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.PriceConditions;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class PriceConditionRepository : ILegacyPriceConditionRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;

        public PriceConditionRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        private async IAsyncEnumerable<PriceCondition> GetAllAsync(Expression<Func<PriceConditionEF, bool>> filter, int? take, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var query = _db.PriceConditions
                .AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

#if DEBUG
            take ??= 1000;
#endif
            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            var priceConditionsEf = await query.ToListAsync(cancellationToken);

            foreach (var item in priceConditionsEf)
            {
                yield return await MapToDomain(item, cancellationToken);
            }
        }

        public async IAsyncEnumerable<PriceCondition> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var item in GetAllAsync(null, null, cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<PriceCondition> GetAsync(int id, CancellationToken cancellationToken)
        {
            await foreach (var item in GetAllAsync(p => p.Id == id, null, cancellationToken))
            {
                return item;
            }

            return null;
        }

        private async Task<PriceCondition> MapToDomain(PriceConditionEF item, CancellationToken cancellationToken)
        {
            var priceConditionMap = await _mapDb.PriceConditionMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == item.Id, cancellationToken);
            var clientMapEf = await _mapDb.ClientMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == item.ClientId, cancellationToken);
            var productTypeMapEf = await _mapDb.ProductTypeMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == item.ProductTypeId, cancellationToken);
            var vendorMap = await _mapDb.ManufacturerMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyTitle == item.Vendor, cancellationToken);

            return new PriceCondition(
                new IdMap(item.Id, priceConditionMap?.ErpGuid),
                item.Date,
                item.ClientId.HasValue && item.ClientId > 0
                    ? new IdMap(item.ClientId.Value, clientMapEf?.ErpGuid)
                    : null,
                item.ProductTypeId.HasValue && item.ProductTypeId > 0
                    ? new IdMap(item.ProductTypeId.Value, productTypeMapEf?.ErpGuid)
                    : null,
                vendorMap == null ? null : new IdMap(vendorMap.LegacyId, vendorMap.ErpGuid),
                item.ProductManager,
                item.PriceType,
                item.DateTo,
                item.Comment,
                item.Value,
                item.PercentValue,
                item.UpperThresholdPriceType
            );
        }
    }
}