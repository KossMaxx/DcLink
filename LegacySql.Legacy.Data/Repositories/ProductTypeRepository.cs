using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.ProductTypeCategoryGroups;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.ProductTypeCategoryParameters;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ProductTypeRepository : ILegacyProductTypeRepository
    {
        private LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        private List<int> _notUsedTypesIds;

        public ProductTypeRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
            _notUsedTypesIds = GetNotUsedTypesIds();
        }
        
        public List<int> GetNotUsedTypesIds()
        {
            //List<int> notUsedTypesIds = new List<int>(){129, 247, 431, 502, 798, 805, 828, 833, 889, 1022, 101, 102, 177};
            
            var notUsedTypesIds2 = _db.ProductTypes
                .Where(t => t.Status == 1)
                .Select(t => t.Code)
                .ToList();
            
            //notUsedTypesIds.AddRange(notUsedTypesIds2);
            
            return notUsedTypesIds2.ToList();
        }

        private async Task<(IEnumerable<ProductType> productType, DateTime? lastDate)> GetAllAsync(CancellationToken cancellationToken)
        {
            var typesEf = await _db.ProductTypes
                .Include(n => n.Categories).ThenInclude(c => c.Group)
                .Include(n => n.Categories).ThenInclude(c => c.Parameters)
                .Where(t => t.Status != 1)
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            var lastDate = GetLastDate(typesEf);
            var types = typesEf.Select(async t => await MapToDomain(t, cancellationToken)).Select(t => t.Result);

            return (types, lastDate);
        }

        public async Task<(IEnumerable<ProductType> productType, DateTime? lastDate)> GetChangedProductTypesAsync(DateTime? changedAt, CancellationToken cancellationToken)
        {
            if (!changedAt.HasValue)
            {
                return await GetAllAsync(cancellationToken);
            }
            
            var productTypesEf = await _db.ProductTypes
                .Include(n => n.Categories).ThenInclude(c => c.Group)
                .Include(n => n.Categories).ThenInclude(c => c.Parameters)
                .Where(t => t.Status != 1)
                .Where(p => p.LastChangeDate.HasValue && p.LastChangeDate > changedAt)
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            var lastDate = GetLastDate(productTypesEf);
            var types = productTypesEf.Select(async p => await MapToDomain(p, cancellationToken)).Select(p => p.Result);

            return (types, lastDate);
        }

        public async Task<ProductType> Get(int id, CancellationToken cancellationToken)
        {
            var productTypeEf = await _db.ProductTypes
                .Include(n => n.Categories).ThenInclude(c => c.Group)
                .Include(n => n.Categories).ThenInclude(c => c.Parameters)
                .FirstOrDefaultAsync(pt => pt.Code == id, cancellationToken);
            return productTypeEf == null ? null : await MapToDomain(productTypeEf, cancellationToken);
        }

        private async Task<ProductType> MapToDomain(ProductTypeEF productTypeEf, CancellationToken cancellationToken)
        {
            var productTypeMap = await _mapDb.ProductTypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == productTypeEf.Code, cancellationToken: cancellationToken);
            return new ProductType(
                new IdMap(productTypeEf.Code, productTypeMap?.ErpGuid),
                productTypeEf.Name,
                productTypeEf.FullName,
                await IsGroupe(productTypeEf.Code, cancellationToken),
                productTypeEf.Web,
                productTypeEf.TypeNameUkr,
                productTypeEf.MainId,
                productTypeMap != null,
                productTypeEf.Categories.Select(async c => await GetCategory(c, cancellationToken)).Select(c => c.Result)
            );
        }

        private async Task<ProductTypeCategory> GetCategory(ProductTypeCategoryEF categoryEf, CancellationToken cancellationToken)
        {
            var productTypeCategoryMap = await _mapDb.ProductTypeCategoryMaps.FirstOrDefaultAsync(c => c.LegacyId == categoryEf.Id, cancellationToken);
            var groupMap = categoryEf.Group == null ? null : await _mapDb.ProductTypeCategoryGroupMaps.FirstOrDefaultAsync(c => c.LegacyId == categoryEf.Group.Id, cancellationToken);
            return new ProductTypeCategory(
                new IdMap(categoryEf.Id, productTypeCategoryMap?.ErpGuid),
                categoryEf.Name,
                categoryEf.NameUA,
                categoryEf.Web,
                categoryEf.Web2,
                categoryEf.PriceTag,
                categoryEf.Group == null ? null : new ProductTypeCategoryGroup(new IdMap(categoryEf.Group.Id, productTypeCategoryMap?.ErpGuid), categoryEf.Group.Name, categoryEf.Group.NameUA, categoryEf.Group.Sort, groupMap != null),
                productTypeCategoryMap != null,
                categoryEf.Parameters.Select(async p => await GetParameter(p, cancellationToken)).Select(p => p.Result)
            );
        }

        private async Task<ProductTypeCategoryParameter> GetParameter(ProductTypeCategoryParameterEF parameterEf, CancellationToken cancellationToken)
        {
            var productTypeCategoryParameterMap = await _mapDb.ProductTypeCategoryParameterMaps.FirstOrDefaultAsync(c => c.LegacyId == parameterEf.Id, cancellationToken);
            return new ProductTypeCategoryParameter(
                new IdMap(parameterEf.Id, productTypeCategoryParameterMap?.ErpGuid),
                parameterEf.Name,
                parameterEf.NameUA,
                productTypeCategoryParameterMap != null
            );
        }

        private async Task<bool> IsGroupe(int code, CancellationToken cancellationToken)
        {
            return await _db.ProductTypes.AnyAsync(t => t.MainId == code, cancellationToken);
        }

        private DateTime? GetLastDate(IEnumerable<ProductTypeEF> types)
        {
            var maxLastChangeDate = types.Max(e => e.LastChangeDate);
            if (!types.Any() || !maxLastChangeDate.HasValue)
            {
                return null;
            }

            return maxLastChangeDate.Value;
        }
    }
}