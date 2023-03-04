using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ClientOrders
{
    public interface ILegacyClientOrderRepository
    {
        IAsyncEnumerable<ClientOrder> GetChangedClientOrdersAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<IEnumerable<ClientOrder>> GetClientOrdersWithNotEndedWarrantyAsync(CancellationToken cancellationToken);
        Task<ClientOrder> GetClientOrderAsync(int id, CancellationToken cancellationToken);
        Task<ClientOrder> GetClientOrderWithAllFiltersAsync(int id, CancellationToken cancellationToken);
    }
}
