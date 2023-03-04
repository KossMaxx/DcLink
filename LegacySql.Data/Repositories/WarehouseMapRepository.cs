using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class WarehouseMapRepository : IWarehouseMapRepository
    {
        private readonly AppDbContext _db;

        public WarehouseMapRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveAsync(WarehouseMap newMap, Guid? id = null)
        {
            WarehouseMapEF mapEf = null;
            if (id.HasValue)
            {
                mapEf = await _db.WarehouseMaps.FirstOrDefaultAsync(m => m.Id == id);
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
                mapEf = await _db.WarehouseMaps.FirstOrDefaultAsync(m =>
                    m.MapGuid == newMap.MapId && m.LegacyId == newMap.LegacyId);
                if (mapEf == null)
                {
                    mapEf = new WarehouseMapEF
                    {
                        MapGuid = newMap.MapId,
                        LegacyId = newMap.LegacyId,
                        ErpGuid = newMap.ExternalMapId
                    };
                    await _db.AddAsync(mapEf);
                }
            }

            await _db.SaveChangesAsync();
            _db.Entry(mapEf).State = EntityState.Detached;
        }

        public async Task<WarehouseMap> GetByLegacyAsync(int legacyId)
        {
            var mapEf = await _db.WarehouseMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == legacyId);
            return Map(mapEf);
        }
        public async Task<WarehouseMap> GetByErpAsync(Guid erpId)
        {
            var mapEf = await _db.WarehouseMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            return Map(mapEf);
        }

        public async Task RemoveByErpAsync(Guid erpId)
        {
            var mapEf = await _db.WarehouseMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            if (mapEf == null)
            {
                throw new KeyNotFoundException($"Маппинга с ErpId: {erpId} не существует");
            }

            _db.WarehouseMaps.Remove(mapEf);
            await _db.SaveChangesAsync();
        }

        public async Task<WarehouseMap> GetFirstMappingAsync()
        {
            var mapEf = await _db.WarehouseMaps.AsNoTracking().FirstOrDefaultAsync();
            return Map(mapEf);
        }

        private WarehouseMap Map(WarehouseMapEF mapEf)
        {
            return mapEf == null ? null : new WarehouseMap(mapEf.MapGuid, mapEf.LegacyId, mapEf.ErpGuid, mapEf.Id);
        }
    }
}
