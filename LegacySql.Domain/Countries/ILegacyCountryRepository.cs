using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Countries
{
    public interface ILegacyCountryRepository
    {
        Task<Country> GetAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<CountryIsoCode>> GetAllIsoCodesAsync();
    }
}
