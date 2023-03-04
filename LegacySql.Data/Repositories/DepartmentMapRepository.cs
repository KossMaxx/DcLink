using LegacySql.Data.Models;
using LegacySql.Domain.Departments;

namespace LegacySql.Data.Repositories
{
    public class DepartmentMapRepository : BaseMapRepository<DepartmentMapEF>, IDepartmentMapRepository
    {
        public DepartmentMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
