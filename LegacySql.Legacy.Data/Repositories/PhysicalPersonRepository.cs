using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.PhysicalPersons;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class PhysicalPersonRepository : ILegacyPhysicalPersonRepository
    {
        private readonly LegacyDbContext _db;
        private readonly AppDbContext _mapDb;
        public PhysicalPersonRepository(LegacyDbContext db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        public async Task<(IEnumerable<PhysicalPerson> persons, DateTime? lastDate)> GetAllAsync(CancellationToken cancellationToken)
        {
            var physicalPersonEf = await _db.PhysicalPersons                
#if DEBUG
                .Take(1000)
#endif
                .ToListAsync(cancellationToken);

            var lastDate = GetLastDate(physicalPersonEf);
            var persons = physicalPersonEf.Select(async i => await MapToDomain(i, cancellationToken)).Select(i => i.Result);

            return (persons, lastDate);
        }

        private async Task<PhysicalPerson> MapToDomain(PhysicalPersonEF physicalPersonsEf, CancellationToken cancellationToken)
        {
            var physicalPersonMap = await _mapDb.PhysicalPersonMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.LegacyId == physicalPersonsEf.Id, cancellationToken: cancellationToken);
            return new PhysicalPerson
            (
                physicalPersonMap != null,
                new IdMap(physicalPersonsEf.Id, physicalPersonMap?.ErpGuid),
                physicalPersonsEf.FirstName,
                physicalPersonsEf.LastName,
                physicalPersonsEf.JobPosition,
                physicalPersonsEf.WorkPhone,
                physicalPersonsEf.PassportSerialNumber,
                physicalPersonsEf.PassportIssuer,
                physicalPersonsEf.PassportSeries,
                physicalPersonsEf.PassportIssuedAt,
                physicalPersonsEf.Email,
                physicalPersonsEf.Fired,
                physicalPersonsEf.NickName,
                physicalPersonsEf.FullName,
                physicalPersonsEf.MiddleName,
                physicalPersonsEf.IndividualTaxNumber,
                physicalPersonsEf.InternalPhone
            );
        }
        private DateTime? GetLastDate(IEnumerable<PhysicalPersonEF> persons)
        {
            var maxLastChangeDate = persons.Max(e => e.ChangedAt);
            if (!persons.Any() || !maxLastChangeDate.HasValue)
            {
                return null;
            }

            return maxLastChangeDate.Value;
        }
    }
}
