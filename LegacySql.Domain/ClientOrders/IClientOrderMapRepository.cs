using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ClientOrders
{
    public interface IClientOrderMapRepository : IEntityMapRepository
    {
        Task<IEnumerable<ExternalMap>> GetRangeByErpAsync(Guid guid);
    }
}
