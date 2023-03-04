using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Departments
{
    public interface ILegacyDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync(IEnumerable<int> notFullMappedIds, CancellationToken cancellationToken);
    }
}
