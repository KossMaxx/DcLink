using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegacySql.Data.Repositories
{
    public class NotFullMappedRepository : INotFullMappedRepository
    {
        protected readonly ILogger<NotFullMappedRepository> _logger;
        private readonly AppDbContext _db;

        public NotFullMappedRepository(AppDbContext db, ILogger<NotFullMappedRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<int>> GetIdsAsync(MappingTypes type)
        {
            return await _db.NotFullMapped
                .Where(i => i.Type == type)
                .AsNoTracking()
                .Select(i => i.InnerId)
                .ToListAsync();
        }

        public async Task RemoveAsync(NotFullMapped mapping)
        {
            var resEf = await _db.NotFullMapped.FirstOrDefaultAsync(i => i.InnerId == mapping.InnerId && i.Type == mapping.Type);
            if (resEf != null)
            {
                _db.NotFullMapped.Remove(resEf);
                await _db.SaveChangesAsync();
            }
        }

        public async Task SaveAsync(NotFullMapped mapping)
        {
            NotFullMappedEF resEf = await _db.NotFullMapped.FirstOrDefaultAsync(i => i.InnerId == mapping.InnerId && i.Type == mapping.Type);
            if (resEf != null)
            {
                resEf.Date = mapping.Date;
                resEf.Why = mapping.Why;
            }
            else
            {
                resEf = new NotFullMappedEF
                {
                    InnerId = mapping.InnerId,
                    Type = mapping.Type,
                    Date = mapping.Date,
                    Why = mapping.Why
                };

                await _db.AddAsync(resEf);
            }

            await _db.SaveChangesAsync();
            _db.Entry(resEf).State = EntityState.Detached;

            _logger.LogWarning($"LegacySql | {mapping.Type} | Mapping: Id: {mapping.InnerId}, {mapping.Why}");
        }
    }
}
