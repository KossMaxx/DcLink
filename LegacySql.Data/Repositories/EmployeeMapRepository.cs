using LegacySql.Data.Models;
using LegacySql.Domain.Employees;

namespace LegacySql.Data.Repositories
{
    public class EmployeeMapRepository : BaseMapRepository<EmployeeMapEF>, IEmployeeMapRepository
    {
        public EmployeeMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
