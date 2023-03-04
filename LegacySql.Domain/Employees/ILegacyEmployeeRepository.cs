using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Employees
{
    public interface ILegacyEmployeeRepository
    {
        Task<(IEnumerable<Employee> employees, DateTime? lastDate)> GetAllAsync(CancellationToken cancellationToken);
    }
}
