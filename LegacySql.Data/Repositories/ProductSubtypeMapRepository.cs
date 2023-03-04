using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.ProductSubtypes;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class ProductSubtypeMapRepository : IProductSubtypeMapRepository
    {
        private readonly AppDbContext _db;
        public ProductSubtypeMapRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveAsync(ProductSubtypeMap newMap, Guid? id = null)
        {
            ProductSubtypeMapEF mapEf = null;
            if (id.HasValue)
            {
                mapEf = await _db.ProductSubtypeMaps.FirstOrDefaultAsync(m => m.Id == id);
                if (mapEf == null)
                {
                    return;
                }
                mapEf.MapGuid = newMap.MapId;
                mapEf.LegacyId = newMap.LegacyId;
                mapEf.ErpGuid = newMap.ExternalMapId;
                mapEf.Title = newMap.Title;
            }
            else
            {
                mapEf = await _db.ProductSubtypeMaps.FirstOrDefaultAsync(m =>
                    m.MapGuid == newMap.MapId && m.LegacyId == newMap.LegacyId);
                if (mapEf == null)
                {
                    mapEf = new ProductSubtypeMapEF
                    {
                        MapGuid = newMap.MapId,
                        LegacyId = newMap.LegacyId,
                        ErpGuid = newMap.ExternalMapId,
                        Title = newMap.Title
                    };
                    await _db.AddAsync(mapEf);
                }
            }

            await _db.SaveChangesAsync();
            _db.Entry(mapEf).State = EntityState.Detached;
        }

        public async Task<ProductSubtypeMap> GetByMapAsync(Guid mapGuid)
        {
            var mapEf = await _db.ProductSubtypeMaps.FirstOrDefaultAsync(m => m.MapGuid == mapGuid);
            return Map(mapEf);
        }

        public async Task<ProductSubtypeMap> GetByLegacyAsync(int legacyId)
        {
            var mapEf = await _db.ProductSubtypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == legacyId);
            return Map(mapEf);
        }

        public async Task<ProductSubtypeMap> GetByErpAsync(Guid erpId)
        {
            var mapEf = await _db.ProductSubtypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            return Map(mapEf);
        }

        public async Task RemoveByErpAsync(Guid erpId)
        {
            var mapEf = await _db.ProductSubtypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            if (mapEf == null)
            {
                throw new KeyNotFoundException($"Маппинга с ErpId: {erpId} не существует");
            }

            _db.ProductSubtypeMaps.Remove(mapEf);
            await _db.SaveChangesAsync();
        }

        private ProductSubtypeMap Map(ProductSubtypeMapEF mapEf)
        {
            return mapEf == null ? null : new ProductSubtypeMap(mapEf.MapGuid, mapEf.LegacyId, mapEf.Title, mapEf.ErpGuid, mapEf.Id);
        }
    }
}
