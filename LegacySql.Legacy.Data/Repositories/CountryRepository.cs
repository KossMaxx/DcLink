using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Countries;
using LegacySql.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class CountryRepository : ILegacyCountryRepository
    {
        private readonly LegacyDbContext _db;
        public CountryRepository(LegacyDbContext db)
        {
            _db = db;
        }
        public async Task<Country> GetAsync(int id, CancellationToken cancellationToken)
        {
            var countryEf = await _db.Countries
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (countryEf == null)
            {
                return null;
            }

            var countryIsoCodeEf = await _db.CountryIsoCodes.FirstOrDefaultAsync(i => i.Id == countryEf.IsoId, cancellationToken);

            return new Country(
                countryEf.Id,
                new CountryIsoCode(countryIsoCodeEf.Id, countryIsoCodeEf.Code),
                countryEf.Title,
                new LanguageId(countryEf.LanguageId)
            );
        }

        public async Task<IEnumerable<CountryIsoCode>> GetAllIsoCodesAsync()
        {
            var countriesEf = await _db.Countries.ToListAsync();
            var isoCodesEf = await _db.CountryIsoCodes.ToListAsync();

            return countriesEf.Select(c => new CountryIsoCode(c.Id, isoCodesEf.FirstOrDefault(i => i.Id == c.IsoId)?.Code));
        }
    }
}
