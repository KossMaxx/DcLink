using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Products;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.RelatedProducts;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class RelatedProductRepository : ILegacyRelatedProductRepository
    {
        private readonly LegacyDbContext _db;
        private readonly IProductMappingResolver _productMappingResolver;
        private readonly List<int> _notUsedTypesIds;
        private readonly int _notFullMappingFilterPortion;

        public RelatedProductRepository(LegacyDbContext db, 
            ILegacyProductTypeRepository legacyProductTypeRepository, int notFullMappingFilterPortion,
            IProductMappingResolver productMappingResolver)
        {
            _db = db;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
            _productMappingResolver = productMappingResolver;
            _notUsedTypesIds = legacyProductTypeRepository.GetNotUsedTypesIds();
        }

        private async IAsyncEnumerable<RelatedProduct> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var relatedProductEfs = await _db.RelatedProducts
                .Include(p => p.MainProduct)
                .Include(p => p.RelatedProduct)
                .Where(GetProductsFilter())
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            foreach (var relatedProduct in relatedProductEfs)
            {
                yield return await MapToDomain(relatedProduct, cancellationToken);
            }
        }

        public async IAsyncEnumerable<RelatedProduct> GetChangedAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (changedAt.HasValue)
            {
                var relatedProductsEf = await _db.RelatedProducts
                    .Include(p => p.MainProduct)
                    .Include(p => p.RelatedProduct)
                    .Where(p => p.LogDate.HasValue && p.LogDate > changedAt)
                    .Where(GetProductsFilter())
#if DEBUG
                    .Take(1000)
#endif
                    .ToListAsync(cancellationToken);

                foreach (var relatedProduct in relatedProductsEf)
                {
                    yield return await MapToDomain(relatedProduct, cancellationToken);
                }

                await foreach (var relatedProduct in GetByNotFullMapping(notFullMappingIds, cancellationToken))
                {
                    yield return relatedProduct;
                }
            }

            await foreach (var relatedProduct in GetAllAsync(cancellationToken))
            {
                yield return relatedProduct;
            }
        }

        public async IAsyncEnumerable<RelatedProduct> GetRelatedProductAsync(int id, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var relatedProductEf = await _db.RelatedProducts
                .Include(p => p.MainProduct)
                .Include(p => p.RelatedProduct)
                .Where(GetProductsFilter())
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (relatedProductEf == null)
            {
                yield return null;
            }

            yield return await MapToDomain(relatedProductEf, cancellationToken);
        }

        private async Task<RelatedProduct> MapToDomain(RelatedProductEF relatedProductEf, CancellationToken cancellationToken)
        {
            return new RelatedProduct(
                relatedProductEf.Id,
                relatedProductEf.MainProductId > 0 ? await GetProductMapping(relatedProductEf.MainProductId, relatedProductEf.MainProduct?.NonCashProductId, cancellationToken) : null,
                relatedProductEf.RelatedProductId > 0 ? await GetProductMapping(relatedProductEf.RelatedProductId, relatedProductEf.RelatedProduct?.NonCashProductId, cancellationToken) : null,
                relatedProductEf.LogDate
            );
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            return new IdMap(productMainSqlId, productErpGuid);
        }

        private async IAsyncEnumerable<RelatedProduct> GetByNotFullMapping(IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
               yield break;
            }

            var result = new List<RelatedProduct>();
            var notFullMappingCount = notFullMappingIds.Count();
            var cycleLimitation = Math.Ceiling((double)notFullMappingCount / _notFullMappingFilterPortion);
            for (var i = 0; i < cycleLimitation; i++)
            {
                var relatedProductEfs = await _db.RelatedProducts
                    .Include(p => p.MainProduct)
                    .Include(p => p.RelatedProduct)
                    .Where(p => notFullMappingIds.Skip(i * _notFullMappingFilterPortion).Take(_notFullMappingFilterPortion).Any(id => id == p.Id))
#if DEBUG
                    .Take(1000)
#endif
                    .AsQueryable()
                    .ToListAsync(cancellationToken);

                foreach (var relatedProduct in relatedProductEfs)
                {
                    yield return await MapToDomain(relatedProduct, cancellationToken);
                }
            }
        }

        private Expression<Func<RelatedProductEF, bool>> GetProductsFilter()
        {
            return p => p.MainProduct.NonCashProductId == null && !_notUsedTypesIds.Contains((int)p.MainProduct.ProductTypeId)
                        && p.RelatedProduct.NonCashProductId == null && !_notUsedTypesIds.Contains((int)p.RelatedProduct.ProductTypeId);
        }
    }
}