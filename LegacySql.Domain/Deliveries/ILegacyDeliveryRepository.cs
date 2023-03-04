using LegacySql.Domain.Deliveries.PublishEntity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Domain.Deliveries
{
    public interface ILegacyDeliveryRepository
    {
        IAsyncEnumerable<Delivery> GetChangedDeliveriesAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken);
        Task<Delivery> GetDeliveryAsync(int id, CancellationToken cancellationToken);
    }
}
