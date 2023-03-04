using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.ProductTypeCategoryGroups;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ProductTypeCategoryGroupRepository : ILegacyProductTypeCategoryGroupRepository
    {
        private LegacyDbContext _db;
        private readonly AppDbContext _mapDb;

        public ProductTypeCategoryGroupRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        public async Task<IEnumerable<ProductTypeCategoryGroup>> GetAllAsync(CancellationToken cancellationToken)
        {
            var groupsEf = await _db.ProductTypeCategoryGroups
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            var groups = groupsEf.Select(async t => await MapToDomain(t, cancellationToken)).Select(t => t.Result);

            return groups;
        }

        public async Task<ProductTypeCategoryGroup> Get(int id, CancellationToken cancellationToken)
        {
            var groupEf = await _db.ProductTypeCategoryGroups
                .FirstOrDefaultAsync(pg => pg.Id == id, cancellationToken);
            return groupEf == null ? null : await MapToDomain(groupEf, cancellationToken);
        }

        private async Task<ProductTypeCategoryGroup> MapToDomain(ProductTypeCategoryGroupEF groupEf, CancellationToken cancellationToken)
        {
            var groupMap = await _mapDb.ProductTypeCategoryGroupMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == groupEf.Id, cancellationToken: cancellationToken);
            return new ProductTypeCategoryGroup(
                new IdMap(groupEf.Id, groupMap?.ErpGuid),
                groupEf.Name,
                groupEf.NameUA,
                groupEf.Sort,
                groupMap != null
                );
        }
    }
}