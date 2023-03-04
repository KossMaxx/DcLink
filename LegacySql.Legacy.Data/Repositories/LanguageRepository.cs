using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Languages;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class LanguageRepository : ILegacyLanguageRepository
    {
        private LegacyDbContext _db;

        public LanguageRepository(LegacyDbContext db)
        {
            _db = db;
        }

        public async Task<Language> GetAsync(int id, CancellationToken cancellationToken)
        {
            var languageEf = await _db.Languages
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (languageEf == null)
            {
                return null;
            }

            return new Language(
                languageEf.Id,
                languageEf.Title
            );
        }
    }
}
