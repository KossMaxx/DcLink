using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ProductSubtypeRepository : ILegacyProductSubtypeRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;

        public ProductSubtypeRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        private async IAsyncEnumerable<ProductSubtype> GetAllAsync(Expression<Func<ProductSubtypeEF, bool>> filter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var query = _db.ProductSubtypes.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var subtypesEf = await query
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken: cancellationToken);

            foreach (var p in subtypesEf)
            {
                yield return await MapToDomain(p, cancellationToken);
            }
        }

        public IAsyncEnumerable<ProductSubtype> GetChangedProductSubtypesAsync(DateTime? changedAt, CancellationToken cancellationToken)
        {
            Expression<Func<ProductSubtypeEF, bool>> filter = PredicateBuilder.New<ProductSubtypeEF>(true);
            if (changedAt.HasValue)
            {
                filter = filter.And(p => p.ChangedAt > changedAt);
            }
            return GetAllAsync(filter, cancellationToken);
        }

        public async Task<ProductSubtype> GetAsync(int id, CancellationToken cancellationToken)
        {
            var subtypesEf = await _db.ProductSubtypes
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (subtypesEf == null)
            {
                return null;
            }

            return await MapToDomain(subtypesEf, cancellationToken);
        }

        private async Task<ProductSubtype> MapToDomain(ProductSubtypeEF productSubtypeEf, CancellationToken cancellationToken)
        {
            var subtypeMap = await _mapDb.ProductSubtypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == productSubtypeEf.Id, cancellationToken: cancellationToken);
            var productTypeMap = await _mapDb.ProductTypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == productSubtypeEf.ProductTypeId, cancellationToken: cancellationToken);
            return new ProductSubtype(
                id: new IdMap(productSubtypeEf.Id, subtypeMap?.ErpGuid),
                title: productSubtypeEf.Title,
                productTypeId: new IdMap(productSubtypeEf.ProductTypeId, productTypeMap?.ErpGuid),
                hasMap: subtypeMap != null,
                changedAt:productSubtypeEf.ChangedAt
            );
        }
    }
}
