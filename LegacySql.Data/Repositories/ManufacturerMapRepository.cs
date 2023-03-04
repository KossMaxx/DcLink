using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.Manufacturer;
using LegacySql.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class ManufacturerMapRepository : IManufacturerMapRepository
    {
        private readonly AppDbContext _db;

        public ManufacturerMapRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveAsync(ManufacturerMap newMap, Guid? id = null)
        {
            ManufacturerMapEF mapEf;
            if (id.HasValue)
            {
                mapEf = await _db.ManufacturerMaps.FirstOrDefaultAsync(m => m.Id == id);
                if (mapEf == null)
                {
                    return;
                }
                mapEf.MapGuid = newMap.MapId;
                mapEf.LegacyId = newMap.LegacyId;
                mapEf.LegacyTitle = newMap.LegacyTitle;
                mapEf.ErpGuid = newMap.ExternalMapId;
            }
            else
            {
                mapEf = await _db.ManufacturerMaps.FirstOrDefaultAsync(m => m.MapGuid == newMap.MapId || m.LegacyId == newMap.LegacyId);
                if (mapEf == null)
                {
                    mapEf = new ManufacturerMapEF
                    {
                        MapGuid = newMap.MapId,
                        LegacyId = newMap.LegacyId,
                        LegacyTitle = newMap.LegacyTitle,
                        ErpGuid = newMap.ExternalMapId
                    };
                    await _db.AddAsync(mapEf);
                }
            }

            await _db.SaveChangesAsync();
            _db.Entry(mapEf).State = EntityState.Detached;
        }

        public async Task<ManufacturerMap> GetByMapAsync(Guid mapGuid)
        {
            var mapEf = await _db.ManufacturerMaps.FirstOrDefaultAsync(m => m.MapGuid == mapGuid);
            return new ManufacturerMap(mapEf.MapGuid, mapEf.LegacyId, mapEf.LegacyTitle, mapEf.ErpGuid, mapEf.Id);
        }

        public async Task<ManufacturerMap> GetByErpAsync(Guid erpGuid)
        {
            var mapEf = await _db.ManufacturerMaps.FirstOrDefaultAsync(m => m.ErpGuid == erpGuid);
            return mapEf == null ? null : new ManufacturerMap(mapEf.MapGuid, mapEf.LegacyId, mapEf.LegacyTitle, mapEf.ErpGuid, mapEf.Id);
        }

        public async Task<IEnumerable<int>> GetAllMapIdAsync()
        {
            return await _db.ManufacturerMaps.Select(m => m.LegacyId).ToListAsync();
        }
    }
}
