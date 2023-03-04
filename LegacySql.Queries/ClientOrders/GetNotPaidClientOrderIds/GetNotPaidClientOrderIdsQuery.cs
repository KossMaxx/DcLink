using MediatR;
using System.Collections.Generic;

namespace LegacySql.Queries.ClientOrders.GetNotPaidClientOrderIds
{
    public class GetNotPaidClientOrderIdsQuery : IRequest<IEnumerable<int>>
    {
        public int ClientId { get; }

        public GetNotPaidClientOrderIdsQuery(int clientId)
        {
            ClientId = clientId;
        }
    }
}
