using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.Cashboxes;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class CashboxMapRepository : ICashboxMapRepository
    {
        private readonly AppDbContext _db;

        public CashboxMapRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveAsync(CashboxMap newMap, Guid? id = null)
        {
            CashboxMapEF mapEf = null;
            if (id.HasValue)
            {
                mapEf = await _db.CashboxMaps.FirstOrDefaultAsync(m => m.Id == id);
                if (mapEf == null)
                {
                    return;
                }
                mapEf.MapGuid = newMap.MapId;
                mapEf.LegacyId = newMap.LegacyId;
                mapEf.ErpGuid = newMap.ExternalMapId;
                mapEf.Description = newMap.Description;
            }
            else
            {
                mapEf = await _db.CashboxMaps.FirstOrDefaultAsync(m =>
                    m.MapGuid == newMap.MapId && m.LegacyId == newMap.LegacyId);
                if (mapEf == null)
                {
                    mapEf = new CashboxMapEF
                    {
                        MapGuid = newMap.MapId,
                        LegacyId = newMap.LegacyId,
                        ErpGuid = newMap.ExternalMapId,
                        Description = newMap.Description
                    };
                    await _db.AddAsync(mapEf);
                }
            }

            await _db.SaveChangesAsync();
            _db.Entry(mapEf).State = EntityState.Detached;
        }

        public async Task<CashboxMap> GetByLegacyAsync(int legacyId)
        {
            var mapEf = await _db.CashboxMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == legacyId);
            return Map(mapEf);
        }

        public async Task RemoveByErpAsync(Guid erpId)
        {
            var mapEf = await _db.CashboxMaps.FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            if (mapEf == null)
            {
                throw new KeyNotFoundException($"Маппинга с ErpId: {erpId} не существует");
            }

            _db.CashboxMaps.Remove(mapEf);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveByLegacyAsync(int legacyId)
        {
            var mapsEf = await _db.CashboxMaps.Where(m => m.LegacyId == legacyId).ToListAsync();
            if (!mapsEf.Any())
            {
                return;
            }

            foreach(var mapEf in mapsEf)
            {
                _db.CashboxMaps.Remove(mapEf);
            }            
            await _db.SaveChangesAsync();
        }

        public async Task<CashboxMap> GetByErpAsync(Guid erpId)
        {
            var mapEf = await _db.CashboxMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            return Map(mapEf);
        }

        private CashboxMap Map(CashboxMapEF mapEf)
        {
            return mapEf == null ? null : new CashboxMap(mapEf.MapGuid, mapEf.LegacyId, mapEf.Description, mapEf.ErpGuid, mapEf.Id);
        }
    }
}
