using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.ProductPriceConditions;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ProductPriceConditionRepository : ILegacyProductPriceConditionRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly IProductMappingResolver _productMappingResolver;

        public ProductPriceConditionRepository(LegacyDbContext db, AppDbContext mapDb, IProductMappingResolver productMappingResolver)
        {
            _db = db;
            _mapDb = mapDb;
            _productMappingResolver = productMappingResolver;
        }

        private async IAsyncEnumerable<ProductPriceCondition> GetAllAsync(Expression<Func<ProductPriceConditionEF, bool>> filter, int? take, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var query = _db.ProductPriceConditions
                .Include(e => e.Product)
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

            var productPriceConditionsEf = await query
                .Select(p => new
                {
                    ItemEF = p,
                    p.Product.Currency,
                })
                .ToListAsync(cancellationToken);

            foreach (var item in productPriceConditionsEf)
            {
                ProductPriceConditionEF itemEf = item.ItemEF;
                itemEf.Currency = item.Currency;
                yield return await MapToDomain(itemEf, cancellationToken);
            }
        }

        public async IAsyncEnumerable<ProductPriceCondition> GetAllAsync([EnumeratorCancellation]CancellationToken cancellationToken)
        {
            await foreach (var item in GetAllAsync(null, null, cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<ProductPriceCondition> GetAsync(int id, CancellationToken cancellationToken)
        {
            await foreach (var pr in GetAllAsync(p => p.Id == id, null, cancellationToken))
            {
                return pr;
            }

            return null;
        }

        private async Task<ProductPriceCondition> MapToDomain(ProductPriceConditionEF item, CancellationToken cancellationToken)
        {
            var productPriceConditionMap = await _mapDb.ProductPriceConditionMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == item.Id, cancellationToken);
            var clientMapEf = await _mapDb.ClientMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == item.ClientId, cancellationToken);

            return new ProductPriceCondition(
                new IdMap(item.Id, productPriceConditionMap?.ErpGuid),
                item.ClientId > 0
                    ? new IdMap(item.ClientId, clientMapEf?.ErpGuid)
                    : null,
                item.ProductId > 0
                    ? await GetProductMapping(
                        item.ProductId,
                        item.Product?.NonCashProductId,
                        cancellationToken)
                    : null,
                item.Price,
                item.DateTo,
                item.Value,
                item.Currency
            );
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            return new IdMap(productMainSqlId, productErpGuid);
        }
    }
}