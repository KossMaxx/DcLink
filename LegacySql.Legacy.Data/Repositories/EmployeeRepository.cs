using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Employees;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class EmployeeRepository : ILegacyEmployeeRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        public EmployeeRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        public async Task<(IEnumerable<Employee> employees, DateTime? lastDate)> GetAllAsync(CancellationToken cancellationToken)
        {
            var employeeEf = await _db.Employees
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            var lastDate = GetLastDate(employeeEf);
            var employees = employeeEf.Select(async i => await MapToDomain(i, cancellationToken)).Select(i => i.Result);

            return (employees, lastDate);
        }

        private async Task<Employee> MapToDomain(EmployeeEF employeeEf, CancellationToken cancellationToken)
        {
            var employeeMap = await _mapDb.EmployeeMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == employeeEf.Id, cancellationToken: cancellationToken);
            return new Employee
            (
                employeeMap != null,
                new IdMap(employeeEf.Id, employeeMap?.ErpGuid),
                employeeEf.FirstName,
                employeeEf.LastName,
                employeeEf.Fired,
                employeeEf.NickName,
                employeeEf.FullName,
                employeeEf.MiddleName,
                employeeEf.IndividualTaxNumber,
                employeeEf.InternalPhone,
                employeeEf.WorkPhone,
                employeeEf.Email,
                employeeEf.PassportSerialNumber,
                employeeEf.PassportIssuer,
                employeeEf.PassportSeries,
                employeeEf.PassportIssuedAt
            );
        }

        private DateTime? GetLastDate(IEnumerable<EmployeeEF> employees)
        {
            var maxLastChangeDate = employees.Max(e => e.ChangedAt);
            if (!employees.Any() || !maxLastChangeDate.HasValue)
            {
                return null;
            }

            return maxLastChangeDate.Value;
        }
    }
}
