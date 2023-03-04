using LegacySql.Data.Models;
using LegacySql.Domain.Clients;

namespace LegacySql.Data.Repositories
{
    public class ClientMapRepository : BaseMapRepository<ClientMapEF>, IClientMapRepository
    {
        public ClientMapRepository(AppDbContext db) : base(db)
        {
        }
    }
}
