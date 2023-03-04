using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacySql.Domain.Manufacturer
{
    public interface ILegacyManufacturerRepository
    {
        Task<IEnumerable<Manufacturer>> GetAllAsync();
        Task<string> GetAsync(int? legacyId);
        Task<int?> GetAsync(string legacyTitle);
    }
}
