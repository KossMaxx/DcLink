using System.Net;
using MediatR;

namespace LegacySql.Tests.ClientOrders.ClientOrdersCreateFillMapping
{
    public class ClientOrdersCreateFillMappingCommand : IRequest
    {
        public ClientOrdersCreateFillMappingCommand(int count)
        {
            Count = count;
        }

        public int Count { get; }
    }
}
