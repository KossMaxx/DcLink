using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Products
{
    public interface IProductMappingResolver
    {
        Task<(int productMainSqlId, Guid? productErpGuid)> ResolveMappingAsync(int productId, int? productCashlessId, CancellationToken cancellationToken);
    }
}
