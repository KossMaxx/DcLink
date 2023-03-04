using LegacySql.Domain.Rates;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LegacySql.Legacy.Data.Repositories
{
    public class RateRepository : ILegacyRateRepository
    {
        private readonly LegacyDbContext _db;

        public RateRepository(LegacyDbContext db)
        {
            _db = db;
        }

        public async IAsyncEnumerable<Rate> GetRatesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var ratesEf = await _db.Rates.ToListAsync(cancellationToken);
            foreach (var item in ratesEf)
            {
                yield return MapToDomain(item);
            }
        }

        private Rate MapToDomain(RateEF rate)
        {
            return new Rate(
                rate.Id,
                rate.Title,
                rate.Value
            );
        }
    }
}
