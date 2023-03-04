using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Classes
{
    public interface ILegacyClassRepository
    {
        IAsyncEnumerable<ProductClass> GetChangedClassAsync(CancellationToken cancellationToken);
        Task<ProductClass> GetAsync(string title, CancellationToken cancellationToken);
    }
}
