using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Languages
{
    public interface ILegacyLanguageRepository
    {
        Task<Language> GetAsync(int id, CancellationToken cancellationToken);
    }
}
