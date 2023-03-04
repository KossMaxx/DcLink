using LegacySql.Data.Models;
using LegacySql.Domain.FreeDocuments;

namespace LegacySql.Data.Repositories
{
    public class FreeDocumentMapRepository : BaseMapRepository<FreeDocumentMapEF>, IFreeDocumentMapRepository
    {
        public FreeDocumentMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
