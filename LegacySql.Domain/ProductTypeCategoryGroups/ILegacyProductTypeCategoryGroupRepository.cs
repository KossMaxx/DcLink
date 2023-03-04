using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ProductTypeCategoryGroups
{
    public interface ILegacyProductTypeCategoryGroupRepository
    {
        Task<IEnumerable<ProductTypeCategoryGroup>> GetAllAsync(CancellationToken cancellationToken);
        Task<ProductTypeCategoryGroup> Get(int id, CancellationToken cancellationToken);
    }
}
