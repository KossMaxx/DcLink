using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.ClientOrders
{
    public interface ILegacyClientOrderArchivalRepository
    {
        Task<IEnumerable<ClientOrder>> GetClientOrdersAsync(CancellationToken cancellationToken);
    }
}
