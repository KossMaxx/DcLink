using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public abstract class BaseMapRepository<TEntity>
    where TEntity : BaseMapModel, new()
    {
        protected readonly AppDbContext _db;

        protected BaseMapRepository(AppDbContext db)
        {
            _db = db;
#if DEBUG
            Debug.WriteLine(_db.GetHashCode());
#endif
        }
        
        public async Task<ExternalMap> GetByMapAsync(Guid mapGuid)
        {
            var mapEf = await _db.Set<TEntity>().FirstOrDefaultAsync(m => m.MapGuid == mapGuid);
            return Map(mapEf);
        }

        public async Task<ExternalMap> GetByLegacyAsync(int legacyId)
        {
            var mapEf = await _db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == legacyId);
            return Map(mapEf);
        }

        public async Task<ExternalMap> GetByErpAsync(Guid erpId)
        {
            var mapEf = await _db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            return Map(mapEf);
        }

        public async Task SaveAsync(ExternalMap newMap, Guid? id = null)
        {
            TEntity mapEf = null;
            if (id.HasValue)
            {
                mapEf = await _db.Set<TEntity>().FirstOrDefaultAsync(m => m.Id == id);
                if (mapEf == null)
                {
                    return;
                }
                mapEf.MapGuid = newMap.MapId;
                mapEf.LegacyId = newMap.LegacyId;
                mapEf.ErpGuid = newMap.ExternalMapId;
            }
            else
            {
                mapEf = await _db.Set<TEntity>().FirstOrDefaultAsync(m => m.MapGuid == newMap.MapId || m.LegacyId == newMap.LegacyId);
                if (mapEf == null)
                {
                    mapEf = new TEntity()
                    {
                        MapGuid = newMap.MapId,
                        LegacyId = newMap.LegacyId,
                        ErpGuid = newMap.ExternalMapId,
                        CreateDate = DateTime.Now
                    };
                    await _db.AddAsync(mapEf);
                }
            }

            await _db.SaveChangesAsync();
            _db.Entry(mapEf).State = EntityState.Detached;
        }

        public async Task<IEnumerable<int>> GetAllLegacyIds()
        {
            return await _db.Set<TEntity>().Select(m => m.LegacyId).ToListAsync();
        }

        public async Task<IEnumerable<ExternalMap>> GetAllAsync()
        {
            return (await _db.Set<TEntity>().AsNoTracking().ToListAsync()).Select(Map);
        }

        public async Task<IEnumerable<ExternalMap>> GetAllFullMappingsAsync()
        {
            return (await _db.Set<TEntity>().AsNoTracking().Where(e=>e.ErpGuid.HasValue).ToListAsync()).Select(Map);
        }

        private protected ExternalMap Map(BaseMapModel mapEf)
        {
            return mapEf == null ? null : new ExternalMap(mapEf.MapGuid, mapEf.LegacyId, mapEf.ErpGuid, mapEf.Id);
        }

        public async Task<bool> IsMappingExist(Guid id)
        {
            return await _db.Set<TEntity>().AnyAsync(e => e.ErpGuid == id);
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            _db.Remove(new TEntity{Id = id});
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExternalMap>> GetRangeByErpIdsAsync(IEnumerable<Guid> data)
        {
            return (await _db.Set<TEntity>().Where(e => data.Any(d => d == (Guid)e.ErpGuid)).ToListAsync()).Select(Map);
        }
    }
}
