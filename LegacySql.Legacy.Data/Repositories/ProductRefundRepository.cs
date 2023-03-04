using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.ProductRefunds;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ProductRefundRepository : ILegacyProductRefundRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private readonly IProductMappingResolver _productMappingResolver;
        private readonly int _notFullMappingFilterPortion;

        public ProductRefundRepository(LegacyDbContext db, AppDbContext mapDb, int notFullMappingFilterPortion, IProductMappingResolver productMappingResolver)
        {
            _db = db;
            _mapDb = mapDb;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
            _productMappingResolver = productMappingResolver;
        }

        private async IAsyncEnumerable<ProductRefund> GetAllAsync(Expression<Func<ProductRefundEF, bool>> filter, int? take, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var productRefundsQuery = _db.ProductRefunds
                .Include(p => p.Items).ThenInclude(i=>i.Product)
                .AsQueryable();

            if (filter != null)
            {
                productRefundsQuery = productRefundsQuery.Where(filter);
            }

            productRefundsQuery = productRefundsQuery
                .Where(p => p.Type == (int) ProductRefundType.Return);

#if DEBUG
            take ??= 1000;
#endif
            if (take.HasValue)
            {
                productRefundsQuery = productRefundsQuery.Take(take.Value);
            }

            var productRefundsEf = await productRefundsQuery.ToListAsync(cancellationToken);

            foreach (var productRefundEf in productRefundsEf)
            {
                yield return await MapToDomain(productRefundEf, cancellationToken);
            }
        }

        public async IAsyncEnumerable<ProductRefund> GetChangedAsync(DateTime? changedAt,
            IEnumerable<int> notFullMappingIds, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Expression<Func<ProductRefundEF, bool>> filter = PredicateBuilder.New<ProductRefundEF>(true);
            if (changedAt.HasValue)
            {
                filter = filter.And(p => p.ChangedAt.HasValue && p.ChangedAt > changedAt);
            }

            await foreach (var productRefund in GetAllAsync(filter, null, cancellationToken))
            {
                yield return productRefund;
            }

            await foreach (var reject in GetByNotFullMapping(notFullMappingIds, cancellationToken))
            {
                yield return reject;
            }
        }

        public async Task<ProductRefund> GetProductRefundAsync(int id, CancellationToken cancellationToken)
        {
            await foreach (var pr in GetAllAsync(p => p.Id == id, null, cancellationToken))
            {
                return pr;
            }

            return null;
        }

        private async Task<ProductRefund> MapToDomain(ProductRefundEF productRefundEf, CancellationToken cancellationToken)
        {
            var clientMapEf = await _mapDb.ClientMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == productRefundEf.ClientId, cancellationToken);

            var productRefundEfMap = await _mapDb.ProductRefundMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == productRefundEf.Id, cancellationToken);

            return new ProductRefund(
                productRefundEfMap != null,
                new IdMap(productRefundEf.Id, productRefundEfMap?.ErpGuid),
                productRefundEf.Date,
                productRefundEf.ChangedAt,
                productRefundEf.ClientId > 0 ? new IdMap(productRefundEf.ClientId, clientMapEf?.ErpGuid) : null,
                GetProductRefundItems(productRefundEf, cancellationToken),
                await GetClientOrderMap(productRefundEf.Id, cancellationToken)
            );
        }

        private async Task<IdMap> GetClientOrderMap(int id, CancellationToken cancellationToken)
        {
            var connectedDocuments = await _db.ConnectedDocuments
                .FirstOrDefaultAsync(e => (e.Doc1Id == id && e.Type1 == (byte)ProductRefundType.Return && e.Type2 == (byte)ProductRefundType.ClientOrder)
                                          || (e.Doc2Id == id && e.Type2 == (byte)ProductRefundType.Return && e.Type1 == (byte)ProductRefundType.ClientOrder), cancellationToken);

            if (connectedDocuments == null)
            {
                return null;
            }

            var clientOrderLegacyId = connectedDocuments.Type1 == (byte) ProductRefundType.Return
                ? connectedDocuments.Doc2Id
                : connectedDocuments.Doc1Id;

            var clientOrderMap = await _mapDb.ClientOrderMaps.AsNoTracking().FirstOrDefaultAsync(e => e.LegacyId == clientOrderLegacyId, cancellationToken);

            return new IdMap(clientOrderLegacyId, clientOrderMap?.ErpGuid);
        }

        private IEnumerable<ProductRefundItem> GetProductRefundItems(ProductRefundEF productRefundEf, CancellationToken cancellationToken)
        {
            return productRefundEf.Items.Select(async i => new ProductRefundItem(
                    i.Quantity,
                    i.ProductId.HasValue && i.ProductId > 0 ? await GetProductMapping(i.ProductId.Value, i.Product.NonCashProductId, cancellationToken) : null,
                    i.Price ?? 0))
                .Select(p => p.Result);
        }

        private async Task<IdMap> GetProductMapping(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var (productMainSqlId, productErpGuid) = await _productMappingResolver.ResolveMappingAsync(productId, productCashlessId, cancellationToken);
            return new IdMap(productMainSqlId, productErpGuid);
        }

        private async IAsyncEnumerable<ProductRefund> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
                yield break;
            }

            var notFullMappingCount = notFullMappingIds.Count();
            var cycleLimitation = (double) notFullMappingCount / _notFullMappingFilterPortion;
            for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
            {
                Expression<Func<ProductRefundEF, bool>> filter =
                    p => notFullMappingIds.Skip(i * _notFullMappingFilterPortion)
                        .Take(_notFullMappingFilterPortion).Any(id => id == p.Id);

                await foreach (var productRefund in GetAllAsync(filter, null, cancellationToken))
                {
                    yield return productRefund;
                }
            }
        }
    }
}