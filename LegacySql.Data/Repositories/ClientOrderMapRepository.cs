using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegacySql.Data.Models;
using LegacySql.Domain.ClientOrders;
using LegacySql.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Data.Repositories
{
    public class ClientOrderMapRepository : BaseMapRepository<ClientOrderMapEF>, IClientOrderMapRepository
    {
        public ClientOrderMapRepository(AppDbContext db) : base(db)
        {}

        public async Task<IEnumerable<ExternalMap>> GetRangeByErpAsync(Guid id)
        {
            return (await _db.ClientOrderMaps.Where(e => e.ErpGuid == id).ToListAsync()).Select(Map);
        }
    }
}
