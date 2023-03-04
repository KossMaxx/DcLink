using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.Classes;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class ClassMapRepository : IClassMapRepository
    {
        private readonly AppDbContext _db;

        public ClassMapRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveAsync(ClassMap newMap, Guid? id = null)
        {
            ClassMapEF mapEf;
            if (id.HasValue)
            {
                mapEf = await _db.ClassMaps.FirstOrDefaultAsync(m => m.Id == id);
                if (mapEf == null)
                {
                    return;
                }
                mapEf.MapGuid = newMap.MapId;
                mapEf.LegacyTitle = newMap.LegacyTitle;
                mapEf.ErpGuid = newMap.ExternalMapId;
            }
            else
            {
                mapEf = await _db.ClassMaps.FirstOrDefaultAsync(m => m.MapGuid == newMap.MapId || m.LegacyTitle == newMap.LegacyTitle);
                if (mapEf == null)
                {
                    mapEf = new ClassMapEF
                    {
                        MapGuid = newMap.MapId,
                        LegacyTitle = newMap.LegacyTitle,
                        ErpGuid = newMap.ExternalMapId
                    };
                    await _db.AddAsync(mapEf);
                }
            }

            await _db.SaveChangesAsync();
            _db.Entry(mapEf).State = EntityState.Detached;
        }

        public async Task<ClassMap> GetByMapAsync(Guid mapGuid)
        {
            var mapEf = await _db.ClassMaps.FirstOrDefaultAsync(m => m.MapGuid == mapGuid);
            return mapEf == null ? null : new ClassMap(mapEf.MapGuid, mapEf.LegacyTitle, mapEf.ErpGuid, mapEf.Id);
        }

        public async Task<IEnumerable<string>> GetAllMapTitlesAsync()
        {
            return await _db.ClassMaps.Select(m => m.LegacyTitle).ToListAsync();
        }

        public async Task<ClassMap> GetByErpAsync(Guid erpGuid)
        {
            var mapEf = await _db.ClassMaps.FirstOrDefaultAsync(m => m.ErpGuid == erpGuid);
            return mapEf == null ? null : new ClassMap(mapEf.MapGuid, mapEf.LegacyTitle, mapEf.ErpGuid, mapEf.Id);
        }
    }
}
