using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.Infrastructure
{
    public static class QueryableExtension
    {
        public static async Task<(IEnumerable<T> items, int totalItems)> GetPageAsync<T>(this IQueryable<T> source, int page, int pageSize) where T : class
        {
            var query = source;

            int totalItems = query.Count();

            if (page > 1)
            {
                query = query.Skip((page - 1) * pageSize);
            }

            if (pageSize >= 0)
            {
                query = query.Take(pageSize);
            }

            return (await query.ToListAsync(), totalItems);
        }
    }
}
