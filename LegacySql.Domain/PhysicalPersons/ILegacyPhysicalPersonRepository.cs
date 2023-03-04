using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.PhysicalPersons
{
    public interface ILegacyPhysicalPersonRepository
    {
        Task<(IEnumerable<PhysicalPerson> persons, DateTime? lastDate)> GetAllAsync(CancellationToken cancellationToken);
    }
}
