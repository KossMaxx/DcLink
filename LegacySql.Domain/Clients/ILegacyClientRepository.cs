using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Clients
{
    public interface ILegacyClientRepository
    {                
        Task<(IEnumerable<Client> clients, DateTime? lastDate)> GetChangedClients(DateTime? changedAt, CancellationToken cancellationToken, IEnumerable<int> notFullMappedIds = null);
        Task<Client> GetClient(int id, CancellationToken cancellationToken);        
    }
}
