using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.ProductTypeCategoryGroups;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class ProductTypeCategoryGroupMapRepository : IProductTypeCategoryGroupMapRepository
    {
        private readonly AppDbContext _db;

        public ProductTypeCategoryGroupMapRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task SaveAsync(ProductTypeCategoryGroupMap newMap, Guid? id = null)
        {
            ProductTypeCategoryGroupMapEF mapEf = null;
            if (id.HasValue)
            {
                mapEf = await _db.ProductTypeCategoryGroupMaps.FirstOrDefaultAsync(m => m.Id == id);
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
                mapEf = await _db.ProductTypeCategoryGroupMaps.FirstOrDefaultAsync(m =>
                    m.MapGuid == newMap.MapId && m.LegacyId == newMap.LegacyId);
                if (mapEf == null)
                {
                    mapEf = new ProductTypeCategoryGroupMapEF
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

        public async Task<ProductTypeCategoryGroupMap> GetByMapAsync(Guid mapGuid)
        {
            var mapEf = await _db.ProductTypeCategoryGroupMaps.FirstOrDefaultAsync(m => m.MapGuid == mapGuid);
            return Map(mapEf);
        }

        public async Task<ProductTypeCategoryGroupMap> GetByLegacyAsync(int legacyId)
        {
            var mapEf = await _db.ProductTypeCategoryGroupMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == legacyId);
            return Map(mapEf);
        }

        public async Task<Guid?> GetErpByLegacyAsync(int legacyId)
        {
            var mapEf = await _db.ProductTypeCategoryGroupMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == legacyId);
            return mapEf.ErpGuid;
        }

        public async Task<ProductTypeCategoryGroupMap> GetByErpAsync(Guid erpId)
        {
            var mapEf = await _db.ProductTypeCategoryGroupMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            return Map(mapEf);
        }

        public async Task RemoveByErpAsync(Guid erpId)
        {
            var mapEf = await _db.ProductTypeCategoryGroupMaps.AsNoTracking().FirstOrDefaultAsync(m => m.ErpGuid == erpId);
            if (mapEf == null)
            {
                throw new KeyNotFoundException($"Маппинга с ErpId: {erpId} не существует");
            }

            _db.ProductTypeCategoryGroupMaps.Remove(mapEf);
            await _db.SaveChangesAsync();
        }

        private ProductTypeCategoryGroupMap Map(ProductTypeCategoryGroupMapEF mapEf)
        {
            return mapEf == null ? null : new ProductTypeCategoryGroupMap(mapEf.MapGuid, mapEf.LegacyId, mapEf.ErpGuid, mapEf.Id);
        }
    }
}
