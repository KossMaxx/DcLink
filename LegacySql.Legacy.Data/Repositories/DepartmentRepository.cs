using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Departments;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class DepartmentRepository : ILegacyDepartmentRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;

        public DepartmentRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        public async Task<IEnumerable<Department>> GetAllAsync(IEnumerable<int> notFullMappedIds, CancellationToken cancellationToken)
        {
            var filter = PredicateBuilder.New<DepartmentEF>(e => e.Title != "Empty");
            if (notFullMappedIds.Any())
            {
                filter = filter.Or(e => notFullMappedIds.Contains(e.Id));
            }

            var departmentsEf = await _db.Departments.Where(filter)
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            return departmentsEf.Select(async i => await MapToDomain(i, cancellationToken)).Select(i => i.Result);
        }

        private async Task<Department> MapToDomain(DepartmentEF departmentEf, CancellationToken cancellationToken)
        {
            var departmentMapping = await _mapDb.DepartmentMaps.AsNoTracking()
                .FirstOrDefaultAsync(e => e.LegacyId == departmentEf.Id, cancellationToken);
            var bossMapping = await _mapDb.EmployeeMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == departmentEf.BossId, cancellationToken: cancellationToken);

            return new Department(
                new IdMap(departmentEf.Id, departmentMapping?.ErpGuid),
                departmentEf.Title,
                departmentEf.Description,
                departmentEf.BossPosition,
                new IdMap(departmentEf.BossId, bossMapping?.ErpGuid),
                departmentMapping != null
            );
        }
    }
}
