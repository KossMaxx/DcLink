using System;
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
    public class ErpNotFullMappedRepository : IErpNotFullMappedRepository
    {
        private readonly AppDbContext _db;
        protected readonly ILogger<ErpNotFullMappedRepository> _logger;

        public ErpNotFullMappedRepository(AppDbContext db, ILogger<ErpNotFullMappedRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        
        public async Task SaveAsync(ErpNotFullMapped mapping)
        {
            var resEf = await _db.ErpNotFullMapped.FirstOrDefaultAsync(i => i.ErpId == mapping.ErpId && i.Type == mapping.Type);
            if (resEf != null)
            {
                resEf.ErpId = mapping.ErpId;
                resEf.Type = mapping.Type;
                resEf.Date = mapping.Date;
                resEf.Why = mapping.Why;
                resEf.Value = mapping.Value;
            }
            else
            {
                resEf = new ErpNotFullMappedEF
                {
                    ErpId = mapping.ErpId,
                    Type = mapping.Type,
                    Date = mapping.Date,
                    Why = mapping.Why,
                    Value = mapping.Value
                };

                await _db.AddAsync(resEf);
            }

            await _db.SaveChangesAsync();
            _db.Entry(resEf).State = EntityState.Detached;

            _logger.LogWarning($"LegacySqlConsumers | {mapping.Type} | Mapping: Id: {mapping.ErpId}, {mapping.Why}");
        }

        public async Task<IEnumerable<ErpNotFullMapped>> GetAllAsync()
        {
            var mappingsEf = await _db.ErpNotFullMapped.ToListAsync();
            return mappingsEf.Select(e => new ErpNotFullMapped(e.Id, e.ErpId, e.Type, e.Date, e.Why, e.Value));
        }

        public async Task RemoveAsync(Guid erpId, MappingTypes type)
        {
            var resEf = await _db.ErpNotFullMapped.FirstOrDefaultAsync(i => i.ErpId == erpId && i.Type == type);
            if (resEf != null)
            {
                _db.ErpNotFullMapped.Remove(resEf);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<ErpNotFullMapped> GetByErpIdAsync(Guid id, MappingTypes type)
        {
            var mappingsEf = await _db.ErpNotFullMapped.FirstOrDefaultAsync(e=>e.ErpId==id && e.Type == type);
            if (mappingsEf == null)
            {
                return null;
            }
            return new ErpNotFullMapped(mappingsEf.Id, mappingsEf.ErpId, mappingsEf.Type, mappingsEf.Date, mappingsEf.Why, mappingsEf.Value);
        }

        public async Task<IEnumerable<ErpNotFullMapped>> GetAllByTypeAsync(MappingTypes type)
        {
            var mappingsEf = await _db.ErpNotFullMapped.Where(e=>e.Type == type).ToListAsync();
            return mappingsEf.Select(e => new ErpNotFullMapped(e.Id, e.ErpId, e.Type, e.Date, e.Why, e.Value));
        }
    }
}
