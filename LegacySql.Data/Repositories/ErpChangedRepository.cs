using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.ErpChanged;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class ErpChangedRepository : IErpChangedRepository
    {
        private readonly AppDbContext _db;

        public ErpChangedRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task Save(int legacyId, DateTime? date, string type)
        {
            var entity = await _db.ErpChanged.FirstOrDefaultAsync(e => e.LegacyId == legacyId && e.Type == type);
            if (entity == null)
            {
                var newEntity = new ErpChangedEF
                {
                    LegacyId = legacyId,
                    Date = date ?? DateTime.Now,
                    Type = type
                };
                await _db.AddAsync(newEntity);
            }
            else
            {
                entity.Date = date ?? DateTime.Now;
            }

            await _db.SaveChangesAsync();
            if (entity != null)
            {
                _db.Entry(entity).State = EntityState.Detached;
            }
        }

        public async Task<IEnumerable<ErpChanged>> GetAll(string type)
        {
            return (await _db.ErpChanged.Where(e => e.Type == type).ToListAsync())
                .Select(e => new ErpChanged(e.LegacyId, e.Date, e.Type, e.Id));
        }

        public async Task Delete(int legacyId, string type)
        {
            var entityEf = await _db.ErpChanged.FirstOrDefaultAsync(e => e.LegacyId == legacyId && e.Type == type);
            if (entityEf != null)
            {
                _db.Remove(entityEf);
                await _db.SaveChangesAsync();
            }
        }
    }
}
