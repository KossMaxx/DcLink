using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.ProductTypes;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class ProductTypeMapRepository : IProductTypeMapRepository
    {
        private readonly AppDbContext _db;

        public ProductTypeMapRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveAsync(ProductTypeMap newMap, Guid? id = null)
        {
            ProductTypeMapEF mapEf = null;
            if (id.HasValue)
            {
                mapEf = await _db.ProductTypeMaps.FirstOrDefaultAsync(m => m.Id == id);
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
                mapEf = await _db.ProductTypeMaps.FirstOrDefaultAsync(m =>
                    m.MapGuid == newMap.MapId && m.LegacyId == newMap.LegacyId);
                if (mapEf == null)
                {
                    mapEf = new ProductTypeMapEF
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

        public async Task<ProductTypeMap> GetByMapAsync(Guid mapGuid)
        {
            var mapEf = await _db.ProductTypeMaps.FirstOrDefaultAsync(m => m.MapGuid == mapGuid);
            return Map(mapEf);
        }

        public async Task<ProductTypeMap> GetByLegacyAsync(int legacyId)
        {
            var mapEf = await _db.ProductTypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == legacyId);
            return Map(mapEf);
        }

        public async Task<Guid?> GetErpByLegacyAsync(int legacyId)
        {
            var mapEf = await _db.ProductTypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == legacyId);
            return mapEf.ErpGuid;
        }

        public async Task<ProductTypeMap> GetByErpAsync(Guid erpId)
        {
            var mapEf = await _db.ProductTypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            return Map(mapEf);
        }

        public async Task RemoveByErpAsync(Guid erpId)
        {
            var mapEf = await _db.ProductTypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            if (mapEf == null)
            {
                throw new KeyNotFoundException($"Маппинга с ErpId: {erpId} не существует");
            }

            _db.ProductTypeMaps.Remove(mapEf);
            await _db.SaveChangesAsync();
        }

        private ProductTypeMap Map(ProductTypeMapEF mapEf)
        {
            return mapEf == null ? null : new ProductTypeMap(mapEf.MapGuid, mapEf.LegacyId, mapEf.ErpGuid, mapEf.Id);
        }
    }
}
