using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ProductMappingResolver : IProductMappingResolver
    {
        private readonly LegacyDbConnection _db;
        private readonly AppDbContext _mapDb;

        public ProductMappingResolver(LegacyDbConnection db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        public async Task<(int productMainSqlId, Guid? productErpGuid)> ResolveMappingAsync(int productId, int? productCashlessId, CancellationToken cancellationToken)
        {
            var productMainSqlId = productId;
            var productErpGuid = (await _mapDb.ProductMaps.AsNoTracking()
                .FirstOrDefaultAsync(e => e.LegacyId == productId, cancellationToken))
                ?.ErpGuid;

            if (productErpGuid.HasValue)
            {
                return (productMainSqlId, productErpGuid);
            }

            if (!productCashlessId.HasValue)
            {
                var cashlessIdSql = @"select beznal_tovID from Товары 
                                    where КодТовара = @ProductId";
                productCashlessId = await _db.Connection.QueryFirstOrDefaultAsync<int?>(cashlessIdSql, new { ProductId = productId });
                if (productCashlessId.HasValue)
                {
                    productErpGuid = (await _mapDb.ProductMaps.AsNoTracking()
                        .FirstOrDefaultAsync(e => e.LegacyId == productCashlessId.Value, cancellationToken))
                        ?.ErpGuid;
                    if (productErpGuid.HasValue)
                    {
                        productMainSqlId = productCashlessId.Value;
                        return (productMainSqlId, productErpGuid);
                    }
                }
            }

            var sql = @"select КодТовара from Товары 
                      where beznal_tovID = @ProductId";
            var cashProductIds = await _db.Connection.QueryAsync<int>(sql, new { ProductId = productId });
            foreach (var cashProductId in cashProductIds)
            {
                productErpGuid = (await _mapDb.ProductMaps.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.LegacyId == cashProductId, cancellationToken))
                    ?.ErpGuid;
                if (productErpGuid.HasValue)
                {
                    productMainSqlId = cashProductId;
                    productErpGuid = productErpGuid.Value;
                    return (productMainSqlId, productErpGuid);
                }
            }

            return (productMainSqlId, null);
        }
    }
}
